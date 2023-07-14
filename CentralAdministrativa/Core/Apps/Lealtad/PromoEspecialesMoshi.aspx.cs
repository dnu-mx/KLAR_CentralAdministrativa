using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Utilidades;
using DALLealtad.BaseDatos;
using DALLealtad.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;

namespace Lealtad
{
    public partial class PromoEspecialesMoshi : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página de Promociones Especiales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {

                }

                if (!X.IsAjaxRequest)
                {
                    this.LlenaComboNivelesLealtad();
                    this.LlenaComboPromociones();
                }
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Llena el combo con los niveles de lealtad a partir del catálogo de base de datos
        /// </summary>
        protected void LlenaComboNivelesLealtad()
        {
            try
            {
                this.StoreNivelLealtad.RemoveAll();

                this.StoreNivelLealtad.DataSource = DAOMoshi.ObtieneCatalogoNivelesLealtad(this.Usuario);
                this.StoreNivelLealtad.DataBind();
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Promociones Especiales", "Ocurrió un error al consultar los Niveles.").Show();
                DALLealtad.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Llena el combo con las promociones especiales del catálogo en base de datos
        /// </summary>
        protected void LlenaComboPromociones()
        {
            try
            {
                this.StorePromocion.RemoveAll();

                this.StorePromocion.DataSource = DAOMoshi.ObtieneCatalogoPromociones(this.Usuario);
                this.StorePromocion.DataBind();
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Promociones Especiales", "Ocurrió un error al consultar las Promociones.").Show();
                DALLealtad.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Controla el evento de selección de un elemento del combo de Nivel de Lealtad
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void select_NivelLealtad(object sender, DirectEventArgs e)
        {
            this.cBoxPromocion.ClearValue();
        }

        /// <summary>
        /// Controla el evento de selección de un elemento del combo de Promoción
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void select_Promocion(object sender, DirectEventArgs e)
        {
            try
            {
                DataTable dtParamsPromo = DAOMoshi.ObtieneParamsPromoEspecial(
                    int.Parse(this.cBoxPromocion.SelectedItem.Value),
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                Int64 IdRegla = Convert.ToInt64(dtParamsPromo.Rows[0]["IdRegla"].ToString());
                Int64 ID_CadenaComercial = Convert.ToInt64(dtParamsPromo.Rows[0]["IdCadenaComercial"].ToString());

                hdnIdReglaPE.Value = IdRegla;
                hdnIdCadena.Value = ID_CadenaComercial;

                LlenaGridPropiedades(ID_CadenaComercial, IdRegla);
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Promociones Especiales", "Ocurrió un error al consultar  los valores de la regla.").Show();
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Llena el PropertyGrid con los parámetros y valores de la regla
        /// </summary>
        /// <param name="ID_CadenaComercial">Identificador de la cadena comercial</param>
        protected void LlenaGridPropiedades(Int64 ID_CadenaComercial, Int64 ID_Regla)
        {
            try
            {
                PropertyGridParameterCollection source = new PropertyGridParameterCollection();
                this.GridPropiedades.SetSource(source);

                foreach (ValorRegla unaProp in DAOMoshi.ListaValoresReglaPromoEspecial(ID_CadenaComercial,
                    int.Parse(this.cBoxPromocion.SelectedItem.Value),
                    int.Parse(this.cBoxNivelLealtad.SelectedItem.Value),
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Values)
                {
                    PropertyGridParameter GridProp = new PropertyGridParameter(unaProp.Descripcion, unaProp.Valor);

                    Dictionary<String, ValorRegla> laRespuesta = DAORegla.ListaDeValoresReglaPredefinidos(ID_Regla, ID_CadenaComercial, unaProp.ID_ValorRegla, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));


                    if (laRespuesta.Count >= 1)
                    {
                        ComboBox comboBox = new ComboBox { ID = unaProp.Nombre.Replace('@', '_'), Editable = false, EmptyText = "Selecciona una Opción...", AllowBlank = false };

                        foreach (ValorRegla unValorEstablecido in laRespuesta.Values)
                        {
                            Ext.Net.ListItem unItem = new Ext.Net.ListItem(unValorEstablecido.Descripcion, unValorEstablecido.Valor);
                            comboBox.Items.Add(unItem);
                        }

                        PropertyGridParameter GridProp2 = new PropertyGridParameter(unaProp.Descripcion, "(Selecciona un Valor)");
                        GridProp2.DisplayName = unaProp.Descripcion;
                        GridProp2.Editor.Add(comboBox);

                        this.GridPropiedades.AddProperty(GridProp2);
                    }

                    else
                    {
                        if (unaProp.Nombre == Configuracion.Get(Guid.Parse(
                                ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveValorReglaDiasSemana").Valor
                            ||
                            unaProp.Nombre == Configuracion.Get(Guid.Parse(
                                ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveValorReglaSucursales").Valor
                            ||
                            unaProp.Nombre == Configuracion.Get(Guid.Parse(
                                ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveValorReglaTipoConsumo").Valor)
                        {

                            MultiCombo mc = new MultiCombo() { ID = unaProp.Nombre.Replace('@', '_') };

                            if (unaProp.Nombre == Configuracion.Get(Guid.Parse(
                                ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveValorReglaDiasSemana").Valor)
                            {
                                for (int dia = 1; dia <= 7; dia++)
                                {
                                    Ext.Net.ListItem li = new Ext.Net.ListItem();
                                    li.Value = dia.ToString();
                                    li.Text = dia.ToString() + "- " + (dia == 1 ? "Domingo" :
                                        dia == 2 ? "Lunes" : dia == 3 ? "Martes" :
                                        dia == 4 ? "Miércoles" : dia == 5 ? "Jueves" :
                                        dia == 6 ? "Viernes" : "Sábado");

                                    mc.Items.Add(li);
                                }
                            }

                            else if (unaProp.Nombre == Configuracion.Get(Guid.Parse(
                                ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveValorReglaSucursales").Valor)
                            {
                                DataSet dsSucursales = DAOMoshi.ObtieneCatalogoSucursales(this.Usuario,
                                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                                for (int sucursal = 0; sucursal < dsSucursales.Tables[0].Rows.Count; sucursal++)
                                {
                                    Ext.Net.ListItem li = new Ext.Net.ListItem();
                                    li.Value = dsSucursales.Tables[0].Rows[sucursal]["ClaveColectiva"].ToString().Trim();
                                    li.Text = dsSucursales.Tables[0].Rows[sucursal]["NombreORazonSocial"].ToString().Trim();

                                    mc.Items.Add(li);
                                }
                            }

                            else
                            {
                                Ext.Net.ListItem li = new Ext.Net.ListItem();
                                li.Value = li.Text = "Local";
                                mc.Items.Add(li);

                                Ext.Net.ListItem li2 = new Ext.Net.ListItem();
                                li2.Value = li2.Text = "Domicilio";
                                mc.Items.Add(li2);
                            }

                            GridProp.Editor.Add(mc);
                        }

                        else
                        {
                            switch (unaProp.TipoDatoJava.ToUpper())
                            {
                                case "FLOAT":
                                    {
                                        TextField tf = new TextField() { ID = unaProp.Nombre.Replace('@', '_') };
                                        tf.Text = unaProp.Valor;
                                        //df.MaskRe = "^[0-9]{1,8}(.[0-9]{0,2})?$";
                                        tf.AllowBlank = false;
                                        GridProp.Editor.Add(tf); break;
                                    }
                                case "STRING":
                                    {
                                        TextField tf_1 = new TextField() { ID = unaProp.Nombre.Replace('@', '_') };
                                        tf_1.Text = unaProp.Valor;
                                        tf_1.AllowBlank = false;
                                        GridProp.Editor.Add(tf_1); break;

                                    }
                                case "DATETIME":
                                case "DATE":
                                    {
                                        DateField df = new DateField() { ID = unaProp.Nombre.Replace('@', '_') };
                                        df.Text = unaProp.Valor;
                                        df.AllowBlank = false;
                                        GridProp.Editor.Add(df); break;
                                    }
                                case "INT":
                                    {
                                        SpinnerField sf = new SpinnerField() { ID = unaProp.Nombre.Replace('@', '_') };
                                        sf.Text = unaProp.Valor;
                                        sf.AllowBlank = false;
                                        GridProp.Editor.Add(sf); break;
                                    }
                                case "MONEY":
                                    {
                                        TextField tf_2 = new TextField() { ID = unaProp.Nombre.Replace('@', '_') };
                                        tf_2.Text = String.Format("{0:c}", float.Parse(unaProp.Valor));
                                        tf_2.AllowBlank = false;
                                        GridProp.Editor.Add(tf_2); break;
                                    }
                            }
                        }
                    }

                    GridProp.DisplayName = unaProp.Descripcion;

                    this.GridPropiedades.AddProperty(GridProp);
                }

                this.GridPropiedades.Render();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                throw new Exception("LlenaGridPropiedades()", ex);
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Guardar en PropertyGrid con los
        /// valores de los parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnGuardar_Click(object sender, DirectEventArgs e)
        {
            try
            {
                DateTime laFechaInicial = new DateTime();
                DateTime laFechaFinal = new DateTime();

                List<ValorRegla> losCambios = new List<ValorRegla>();

                //Obtiene las propiedades que cambiaron
                foreach (PropertyGridParameter param in this.GridPropiedades.Source)
                {
                    if (param.Name.ToUpper().Equals("Fecha de Inicio".Trim().ToUpper()))
                    {
                        try
                        {
                            laFechaInicial = DateTime.Parse(param.Value.ToString());
                        }
                        catch (Exception)
                        {
                            X.Msg.Notify("Promociones Especiales", "<br />  <b>La Fecha de Inicio es obligatoria y debe ser válida</b> <br /> ").Show();
                            return;
                        }
                    }

                    if (param.Name.ToUpper().Equals("Fecha Fin".Trim().ToUpper()))
                    {
                        try
                        {
                            laFechaFinal = DateTime.Parse(param.Value.ToString());
                        }
                        catch (Exception)
                        {
                            X.Msg.Notify("Promociones Especiales", "<br />  <b>La Fecha de Fin es obligatoria y debe ser válida</b> <br /> ").Show();
                            return;
                        }
                    }

                    if (param.Name.ToUpper().Equals("Hora de Inicio".Trim().ToUpper()))
                    {
                        try
                        {
                            DateTime laHoraInicial = DateTime.ParseExact(param.Value.ToString() + ":00",
                                "HH:mm:ss", CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                            X.Msg.Notify("Promociones Especiales", "<br />  <b> La Hora de Inicio debe ser un valor entre las 00:00 y las 23:59 horas</b> <br /> ").Show();
                            return;
                        }
                    }

                    if (param.Name.ToUpper().Equals("Hora de Fin".Trim().ToUpper()))
                    {
                        try
                        {
                            DateTime laHoraFinal = DateTime.ParseExact(param.Value.ToString() + ":00",
                                "HH:mm:ss", CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                            X.Msg.Notify("Promociones Especiales", "<br />  <b> La Hora de Fin debe ser un valor entre las 00:00 y las 23:59 horas</b> <br /> ").Show();
                            return;
                        }
                    }


                    ///////Validaciones para editores tipo lista
                    if (param.Name.Equals(Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), 
                        "VRDiasSemanaValidos").Valor))
                    {
                        if (String.IsNullOrEmpty(param.Value.ToString()))
                        {
                            param.Value = "-1";

                            ValorRegla prop = new ValorRegla() { Nombre = param.Name, Valor = param.Value.ToString() };

                            losCambios.Add(prop);
                        }
                    }

                    if (param.Name.Equals(Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "VRSucursales").Valor))
                    {
                        if (String.IsNullOrEmpty(param.Value.ToString()))
                        {
                            param.Value = "-1";

                            ValorRegla prop = new ValorRegla() { Nombre = param.Name, Valor = param.Value.ToString() };

                            losCambios.Add(prop);
                        }
                    }


                    if (param.Name.Equals(Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "VRFechasEspeciales").Valor))
                    {
                        string laFecha = "";

                        if (String.IsNullOrEmpty(param.Value.ToString()))
                        {
                            param.Value = "-1";

                            ValorRegla prop = new ValorRegla() { Nombre = param.Name, Valor = param.Value.ToString() };

                            losCambios.Add(prop);
                        }
                        else
                        {
                            try
                            {
                                String[] fechas = param.Value.ToString().Split(',');
                                foreach (string fecha in fechas)
                                {
                                    laFecha = fecha;
                                    DateTime.Parse(fecha.Substring(0, 4) + "-" + fecha.Substring(4, 2) + "-" + fecha.Substring(6, 2));
                                }
                            }
                            catch (Exception)
                            {
                                X.Msg.Notify("Promociones Especiales", "<br />  <b>La Fecha Especial " + laFecha + " es una fecha inválida. El formato es AAAAMMDD.</b> <br /> ").Show();
                                return;
                            }
                        }
                    }

                    if (param.Name.Equals(Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "VRTipoConsumo").Valor))
                    {
                        if (String.IsNullOrEmpty(param.Value.ToString()))
                        {
                            param.Value = "-1";

                            ValorRegla prop = new ValorRegla() { Nombre = param.Name, Valor = param.Value.ToString() };

                            losCambios.Add(prop);
                        }
                    }

                    if (param.IsChanged)
                    {
                        ValorRegla unaProp = new ValorRegla() { Nombre = param.Name, Valor = param.Value.ToString() };

                        losCambios.Add(unaProp);
                    }
                }

                //Validación pare evitar errores si el Grid está vacío
                if (losCambios.Count == 0)
                {
                    return;
                }

                ////Se validan fechas
                if (DateTime.Compare(laFechaInicial, laFechaFinal) > 0)
                {
                    X.Msg.Notify("Promociones Especiales", "Modificación de Valores de la Regla <br /><br />  <b> La Fecha Inicial debe ser menor o igual que la Fecha Final </b> <br />  <br /> ").Show();
                    return;
                }

                if (DateTime.Compare(laFechaFinal, DateTime.Now) == 0)
                {
                    X.Msg.Notify("Promociones Especiales", "Modificación de Valores de la Regla <br /><br />  <b> La Fecha Final no puede ser igual a la actual </b> <br />  <br /> ").Show();
                    return;
                }

                //Guardar Valores
                LNMoshi.ModificaValoresReglaPorGpoCuenta(losCambios, Int64.Parse(HttpContext.Current.Session["ID_CadenaMoshi"].ToString()),
                    Int64.Parse(HttpContext.Current.Session["ID_ReglaPromo"].ToString()),
                    Convert.ToInt32(this.cBoxNivelLealtad.SelectedItem.Value),
                    this.Usuario);

                LlenaGridPropiedades(Int64.Parse(HttpContext.Current.Session["ID_CadenaMoshi"].ToString()),
                    Int64.Parse(HttpContext.Current.Session["ID_ReglaPromo"].ToString()));

                X.Msg.Notify("Promociones Especiales", "Modificación de Valores de la Regla <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Notify("Promociones Especiales", "Modificación de Valores de la Regla <br /><br />  <b> D E C L I N A D O </b> <br />  <br /> ").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Cancelar en PropertyGrid con los
        /// valores de los parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnCancelar_Click(object sender, DirectEventArgs e)
        {
            try
            {
                LlenaGridPropiedades(Int64.Parse(HttpContext.Current.Session["ID_CadenaMoshi"].ToString()),
                    Int64.Parse(HttpContext.Current.Session["ID_ReglaPromo"].ToString()));
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                return;
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Nueva del panel de Promoción
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnNuevaPromocion_Click(object sender, DirectEventArgs e)
        {
            this.FormPanelNuevaPromocion.Reset();
            this.WindowNuevaPromocion.Show();
        }

        /// <summary>
        /// Controla el evento Click al botón Aceptar de la ventana de Nueva Promoción
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnAceptaNuevaPromo_Click(object sender, DirectEventArgs e)
        {
            try
            {
                this.FormPanelNuevaPromocion.Reset();
                this.WindowNuevaPromocion.Hide();

                DataSet dsEvento = new DataSet();
                long IdCadenaComercial;
                int IdEventoOrigen;
                string ClaveNuevoEvento;

                //Se consultan los datos faltantes del nuevo evento
                dsEvento = DAOMoshi.ObtieneDetallesEvento(this.Usuario);

                if (dsEvento.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Promociones Especiales", "Ocurrió un error al consultar datos para la nueva promoción.").Show();
                    return;
                }
                else
                {
                    IdEventoOrigen = int.Parse(dsEvento.Tables[0].Rows[0]["ID_Evento"].ToString().Trim());
                    ClaveNuevoEvento = dsEvento.Tables[0].Rows[0]["ClaveEvento"].ToString().Trim();
                    IdCadenaComercial = long.Parse(dsEvento.Tables[0].Rows[0]["ID_CadenaComercial"].ToString().Trim());
                }

                //Se solicita la creación del nuevo evento
                DAOMoshi.InsertaNuevaPromocionEspecial(IdEventoOrigen, ClaveNuevoEvento,
                    this.txNuevaPromocion.Text, IdCadenaComercial, this.Usuario);

                X.Msg.Notify("Promociones Especiales", "Nueva promoción creada <br /><br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaComboPromociones();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Promociones Especiales", "Ocurrió un error al crear la nueva promoción.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Cancelar de la ventana de Nueva Promoción
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnCancelaNuevaPromo_Click(object sender, DirectEventArgs e)
        {
            this.FormPanelNuevaPromocion.Reset();
            this.WindowNuevaPromocion.Hide();
        }
    }
}