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
    public partial class AdministrarPromosPrana : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Administrar Promociones para Prana
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    cBoxMultiRandoEdad.Selectable = true;
                    LlenaCombosCadena();

                    this.StoreGiros.DataSource = DAOEcommercePrana.ListaGiros(this.Usuario);
                    this.StoreGiros.DataBind();

                    this.StorePresencia.DataSource = DAOEcommercePrana.ListaPresencias(this.Usuario);
                    this.StorePresencia.DataBind();

                    this.StoreClasif.DataSource = DAOEcommercePrana.ListaClasificaciones(this.Usuario);
                    this.StoreClasif.DataBind();

                    this.StoreGenero.DataSource = DAOEcommercePrana.ListaGenero(this.Usuario);
                    this.StoreGenero.DataBind();

                    this.StoreRangoEdad.DataSource = DAOEcommercePrana.ListaRangoEdad(this.Usuario);
                    this.StoreRangoEdad.DataBind();

                    this.StoreTipoRedencion.DataSource = DAOEcommercePrana.ListaTipoRedencion(this.Usuario);
                    this.StoreTipoRedencion.DataBind();

                    this.StorePromoPlus.DataSource = DAOEcommercePrana.ListaPromoPlus(this.Usuario);
                    this.StorePromoPlus.DataBind();
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Solicita la información de cadenas a base de datos, llenando los combos
        /// destinados a esa información
        /// </summary>
        protected void LlenaCombosCadena()
        {
            try
            {
                DataSet dsCadenas = DAOEcommercePrana.ListaCadenas(this.Usuario);

                List<CadenaComboPredictivo> cadenaList = new List<CadenaComboPredictivo>();

                foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
                {
                    var cadenaCombo = new CadenaComboPredictivo()
                    {
                        ID_Cadena = Convert.ToInt64(cadena["ID_Cadena"].ToString()),
                        ClaveCadena = cadena["ClaveCadena"].ToString(),
                        NombreComercial = cadena["NombreComercial"].ToString()
                    };
                    cadenaList.Add(cadenaCombo);
                }

                StoreCadEcom.DataSource = cadenaList;
                StoreCadEcom.DataBind();

                StoreClaveCadenas.DataSource = cadenaList;
                StoreClaveCadenas.DataBind();

            }
            catch (CAppException err)
            {
                throw new Exception("CAppException_LlenaCombosCadena()", err);
            }
            catch (Exception ex)
            {
                throw new Exception("LlenaCombosCadena()", ex);
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
            LimpiaVentanasAdd();

            this.txtClavePromo.Reset();
            this.txtPromocion.Reset();
            StorePromos.RemoveAll();

            LimpiaSeleccionPrevia();
            PanelCentral.Disabled = true;
        }

        /// <summary>
        /// Limpia los controles de las ventanas pop up para añadir nuevas promociones o cadenas
        /// </summary>
        protected void LimpiaVentanasAdd()
        {
            FormPanelNP.Reset();
            this.cBoxClaveCadena.Reset();
            this.cBoxCadena.Reset();
            this.txtClavePromo_Alta.Reset();
            this.txtDescripcionPromo.Reset();
            this.txtPalabrasClave.Reset();

            FormPanelNC.Reset();
            this.txtClaveCadena.Reset();
            this.txtCadena.Reset();
            this.cBoxGiro.Reset();
            this.cBoxPresencia.Reset();
            this.txtFacebook.Reset();
            this.txtWeb.Reset();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de
        /// una promoción en el Grid de Resultados
        /// </summary>
        protected void LimpiaSeleccionPrevia()
        {
            PanelCentral.SetTitle("_");

            FormPanelInfoAd.Reset();
            btnGuardarInfoAd.Disabled = true;

            FormPanelProgramas.Reset();
            cBoxTwist.SelectedIndex = 1;
            cBoxTerra.SelectedIndex = 1;
            cBoxPurina.SelectedIndex = 1;
            cBoxEdenred.SelectedIndex = 1;
            cBoxSams_Benefits.SelectedIndex = 1;
            cBoxSams_Plus.SelectedIndex = 1;
            cBoxCuponClick.SelectedIndex = 1;
            cBoxBoxito.SelectedIndex = 1;
            cBoxBroxel.SelectedIndex = 1;
            cBoxBioBox.SelectedIndex = 1;
            cBoxAdvantage.SelectedIndex = 1;
            cBoxSixtynine.SelectedIndex = 1;
            cBoxBonnus.SelectedIndex = 1;
            cBoxSantander_Affluent.SelectedIndex = 1;
            cBoxCC_Royalty.SelectedIndex = 1;
            cBoxCC_Bets.SelectedIndex = 1;
            cBoxBeneful.SelectedIndex = 1;
            cBoxEdoMex.SelectedIndex = 1;
            cBoxSmartGift.SelectedIndex = 1;
            cBoxBacalar.SelectedIndex = 1;
            cBoxMasPaMi.SelectedIndex = 1;
            cBoxAirPak.SelectedIndex = 1;
            cBoxParco.SelectedIndex = 1;
            cBoxYourPayChoice.SelectedIndex = 1;

            btnGuardaProgramas.Disabled = true;
            FormPanelInfoAd.Show();
        }

        /// <summary>
        /// Controla el evento Click al botón Crear Promoción
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnNuevaPromo_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtNuevaCad = LNEcommercePrana.CreaNuevaPromoInactiva(
                    Convert.ToInt32(this.cBoxCadena.SelectedItem.Value), this.txtClavePromo_Alta.Text,
                    this.txtDescripcionPromo.Text, this.txtPalabrasClave.Text, this.Usuario);

                string msj = dtNuevaCad.Rows[0]["Mensaje"].ToString();
                int idNuevaPromo = Convert.ToInt32(dtNuevaCad.Rows[0]["IdNuevaPromo"]);

                if (idNuevaPromo == -1)
                {
                    X.Msg.Alert("Nueva Promoción", msj).Show();
                }
                else
                {
                    WdwNuevaPromo.Hide();

                    txtClavePromo.Text = txtClavePromo_Alta.Text;
                    LlenaGridResultados();

                    X.Msg.Alert("Nueva Promoción", "<br />" + msj + "<br /> <b> E X I T O S A M E N T E </b> <br /> ",
                        new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "AdminPromosPrana.CargaNuevaPromo()",
                                Text = "Aceptar"
                            }
                        }).Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nueva Promoción", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Nueva Promoción", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento clic al botón Aceptar al mensaje informativo de creación
        /// de promoción exitosa
        /// </summary>
        [DirectMethod(Namespace = "AdminPromosPrana")]
        public void CargaNuevaPromo()
        {
            RowSelectionModel rsm = GridResultados.GetSelectionModel() as RowSelectionModel;
            rsm.SelectedRows.Add(new SelectedRow(0));
            rsm.UpdateSelection();

            GridResultados.FireEvent("RowClick");
        }

        /// <summary>
        /// Controla el evento Click al botón Crear Cadena
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddCadena_Click(object sender, EventArgs e)
        {
            try
            {
                Cadena cad = new Cadena();

                cad.ClaveCadena = this.txtClaveCadena.Text;
                cad.NombreComercial = this.txtCadena.Text;
                cad.ID_Giro = Convert.ToInt32(this.cBoxGiro.SelectedItem.Value);
                cad.ID_Presencia = Convert.ToInt32(this.cBoxPresencia.SelectedItem.Value);
                cad.Facebook = this.txtFacebook.Text;
                cad.Web = this.txtWeb.Text;

                DataTable dtNuevaCad = LNEcommercePrana.CreaNuevaCadena(cad, this.Usuario);

                string msj = dtNuevaCad.Rows[0]["Mensaje"].ToString();
                int idNuevaCadena = Convert.ToInt32(dtNuevaCad.Rows[0]["IdNuevaCadena"]);

                if (idNuevaCadena == -1)
                {
                    X.Msg.Alert("Nueva Cadena", msj).Show();
                }
                else
                {
                    WdwNuevaCadena.Hide();

                    X.Msg.Alert("Nueva Cadena", "<br />" + msj + "<br /> <b> E X I T O S A M E N T E </b> <br /> ");

                    LlenaCombosCadena();

                    cBoxCadena.SetValue(idNuevaCadena);
                    cBoxCadena.SelectedItem.Text = cad.NombreComercial;

                    cBoxClaveCadena.SetValue(idNuevaCadena);
                    cBoxClaveCadena.SelectedItem.Text = cad.ClaveCadena;
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
        /// Controla el evento Click al botón Buscar del panel izquierdo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenaGridResultados();
        }

        /// <summary>
        /// Llena el grid de resultados de promociones con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            try
            {
                DataSet dsPromociones = DAOEcommercePrana.ObtienePromosPorClaveODescripcion(
                    this.txtClavePromo.Text, this.txtPromocion.Text, this.Usuario);

                if (dsPromociones.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Promociones", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StorePromos.DataSource = dsPromociones;
                    StorePromos.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Promociones", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Promociones", ex.Message).Show();
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
                int IdPromo = 0;
                string ClavePromo = "", DescripcionPromo = "";

                LimpiaSeleccionPrevia();

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] promocion = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in promocion[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Promocion": IdPromo = int.Parse(column.Value); break;
                        case "ClavePromocion": ClavePromo = column.Value; break;
                        case "Descripcion": DescripcionPromo = column.Value; break;
                        default:
                            break;
                    }
                }

                this.hdnIdPromocion.Value = IdPromo;

                LlenaFormPanelInfoAd(IdPromo, ClavePromo);
                LlenaFormPanelProgramas(IdPromo);

                PanelCentral.SetTitle(ClavePromo + " - " + DescripcionPromo);
                PanelCentral.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Promociones", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                X.Msg.Alert("Promociones", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el panel de Información Adicional con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdPromocion">Identificador de la promoción</param>
        /// <param name="ClavePromocion">Clave de la promoción</param>
        protected void LlenaFormPanelInfoAd(int IdPromocion, string ClavePromocion)
        {
            try
            {
                Promocion laPromocion = DAOEcommercePrana.ObtieneInfoAdicionalPromocion(IdPromocion, this.Usuario);

                txtClaveCad.Text = laPromocion.ClaveCadena;
                txtClaveProm.Text = ClavePromocion;
                txtCad.Text = laPromocion.Cadena;
                txtTituloPromo.Text = laPromocion.TituloPromocion;
                txtTipoDescuento.Text = laPromocion.TipoDescuento;
                txtDescBenef.Text = laPromocion.Descripcion; 
                txtRestricciones.Text = laPromocion.Restricciones;
                txtPalabraClaveInfo.Text = laPromocion.PalabrasClave;

                cBoxHotDeal.Value = laPromocion.EsHotDeal;
                txtCarrusel.Text = laPromocion.CarruselHome.ToString();
                txtPromoHome.Text = laPromocion.PromoHome.ToString();
                txtOrden.Text = laPromocion.Orden.ToString();

                dfFIniVigPromo.Value = laPromocion.VigenciaInicio;
                dfFFinVigPromo.Value = laPromocion.VigenciaFin;

                cBoxActiva.Value = laPromocion.Activa;
                cBoxClasificacion.Value = laPromocion.ID_Clasificacion;

                cBoxGenero.Value = laPromocion.ID_Genero;
                cBoxTipoRedencion.Value = laPromocion.ID_TipoRedencion;
                cBoxPromocionPlus.Value = laPromocion.ID_PromoPlus;
                txtURLCupon.Text = laPromocion.URLCupon;


                this.cBoxMultiRandoEdad.SelectedItems.Clear();

                foreach (var item in laPromocion.RangosEdad)
                {
                    SelectedListItem select = new SelectedListItem(item.ToString());
                    cBoxMultiRandoEdad.SelectedItems.Add(select);
                }

                cBoxMultiRandoEdad.UpdateSelection();
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
        /// a la actualización de datos de la promoción en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarInfoAd_Click(object sender, EventArgs e)
        {
            try
            {
                Promocion promo = new Promocion();

                promo.Id_Promocion = Convert.ToInt32(this.hdnIdPromocion.Value);
                promo.TituloPromocion = this.txtTituloPromo.Text;
                promo.TipoDescuento = this.txtTipoDescuento.Text;
                promo.Descripcion = this.txtDescBenef.Text;
                promo.Restricciones = this.txtRestricciones.Text;
                promo.EsHotDeal = Convert.ToInt32(cBoxHotDeal.SelectedItem.Value);
                promo.CarruselHome = Convert.ToInt32(this.txtCarrusel.Text);
                promo.PromoHome = Convert.ToInt32(this.txtPromoHome.Text);
                promo.Orden = Convert.ToInt32(txtOrden.Text);
                promo.VigenciaInicio = Convert.ToDateTime(dfFIniVigPromo.SelectedDate);
                promo.VigenciaFin = Convert.ToDateTime(dfFFinVigPromo.SelectedDate);
                promo.Activa = Convert.ToInt32(cBoxActiva.SelectedItem.Value);
                promo.ID_Clasificacion = Convert.ToInt32(cBoxClasificacion.SelectedItem.Value);
                promo.PalabrasClave = this.txtPalabraClaveInfo.Text;

                promo.URLCupon = this.txtURLCupon.Text;

                if (cBoxGenero.SelectedItem.Value == null)
                {
                    promo.ID_Genero = null;
                } else
                {
                    promo.ID_Genero = Convert.ToInt32(cBoxGenero.SelectedItem.Value);
                }

                if (cBoxPromocionPlus.SelectedItem.Value == null)
                {
                    promo.ID_PromoPlus = null;
                }
                else
                {
                    promo.ID_PromoPlus = Convert.ToInt32(cBoxPromocionPlus.SelectedItem.Value);
                }

                if (cBoxTipoRedencion.SelectedItem.Value == null)
                {
                    promo.ID_TipoRedencion = null;
                }
                else
                {
                    promo.ID_TipoRedencion = Convert.ToInt32(cBoxTipoRedencion.SelectedItem.Value);
                }

                Ext.Net.SelectedListItemCollection lista = cBoxMultiRandoEdad.SelectedItems;
                promo.RangosEdad = new List<int>();

                foreach (var item in lista)
                {
                    promo.RangosEdad.Add(Convert.ToInt32(item.Value));
                }
                LNEcommercePrana.ModificaPromocion(promo, this.Usuario);

                X.Msg.Notify("", "Promoción Actualizada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Promoción", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Actualización de Promoción", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el panel de Información Adicional con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdPromocion">Identificador de la promoción</param>
        protected void LlenaFormPanelProgramas(int IdPromocion)
        {
            try
            {
                string token = "cBox";

                DataSet dsProgramas = DAOEcommercePrana.ObtieneProgramasPromocion(
                    IdPromocion, this.Usuario);

                foreach (DataRow dr in dsProgramas.Tables[0].Rows)
                {
                    X.GetCmp<ComboBox>(token + dr["Clave"].ToString()).Value = 1;
                }

                btnGuardaProgramas.Disabled = false;
            }
            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Programas", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Programas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del panel de Programas, llamando a la
        /// actualización los programas asociados a la promoción en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardaProgramas_Click(object sender, EventArgs e)
        {
            try
            {
                List<Programa> losProgramas = new List<Programa>();

                losProgramas.Add(new Programa
                    {
                        ClavePrograma = "Twist",
                        Activo = Convert.ToInt32(cBoxTwist.SelectedItem.Value)
                    });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "Terra",
                       Activo = Convert.ToInt32(cBoxTerra.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "Purina",
                       Activo = Convert.ToInt32(cBoxPurina.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "Edenred",
                       Activo = Convert.ToInt32(cBoxEdenred.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "Sams_Benefits",
                       Activo = Convert.ToInt32(cBoxSams_Benefits.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "Sams_Plus",
                       Activo = Convert.ToInt32(cBoxSams_Plus.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "CuponClick",
                       Activo = Convert.ToInt32(cBoxCuponClick.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "Boxito",
                       Activo = Convert.ToInt32(cBoxBoxito.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "Broxel",
                       Activo = Convert.ToInt32(cBoxBroxel.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "BioBox",
                       Activo = Convert.ToInt32(cBoxBioBox.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "Advantage",
                       Activo = Convert.ToInt32(cBoxAdvantage.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "Sixtynine",
                       Activo = Convert.ToInt32(cBoxSixtynine.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "Bonnus",
                       Activo = Convert.ToInt32(cBoxBonnus.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "Santander_Affluent",
                       Activo = Convert.ToInt32(cBoxSantander_Affluent.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "CC_Royalty",
                       Activo = Convert.ToInt32(cBoxCC_Royalty.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "CC_Bets",
                       Activo = Convert.ToInt32(cBoxCC_Bets.SelectedItem.Value)
                   });
                losProgramas.Add(new Programa
                   {
                       ClavePrograma = "Beneful",
                       Activo = Convert.ToInt32(cBoxBeneful.SelectedItem.Value)
                   });

                losProgramas.Add(new Programa
                {
                    ClavePrograma = "EdoMex",
                    Activo = Convert.ToInt32(cBoxEdoMex.SelectedItem.Value)
                });
                losProgramas.Add(new Programa
                {
                    ClavePrograma = "SmartGift",
                    Activo = Convert.ToInt32(cBoxSmartGift.SelectedItem.Value)
                });
                losProgramas.Add(new Programa
                {
                    ClavePrograma = "Bacalar",
                    Activo = Convert.ToInt32(cBoxBacalar.SelectedItem.Value)
                });
                losProgramas.Add(new Programa
                {
                    ClavePrograma = "MasPaMi",
                    Activo = Convert.ToInt32(cBoxMasPaMi.SelectedItem.Value)
                });
                losProgramas.Add(new Programa
                {
                    ClavePrograma = "AirPak",
                    Activo = Convert.ToInt32(cBoxAirPak.SelectedItem.Value)
                });
                losProgramas.Add(new Programa
                {
                    ClavePrograma = "Parco",
                    Activo = Convert.ToInt32(cBoxParco.SelectedItem.Value)
                });
                losProgramas.Add(new Programa
                {
                    ClavePrograma = "YourPayChoice",
                    Activo = Convert.ToInt32(cBoxYourPayChoice.SelectedItem.Value)
                });

                LNEcommercePrana.ModificaProgramasPromocion(Convert.ToInt32(this.hdnIdPromocion.Value),
                    losProgramas, this.Usuario);

                X.Msg.Notify("", "Programas Actualizados" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Programas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Actualización de Programas", ex.Message).Show();
            }
        }
    }
}
       