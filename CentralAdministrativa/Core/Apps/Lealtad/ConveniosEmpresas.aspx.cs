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
    public partial class ConveniosEmpresas : DALCentralAplicaciones.PaginaBaseCAPP
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
                    this.LlenaComboEmpresas();
                }
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Llena el combo con las empresas a partir del catálogo de base de datos
        /// </summary>
        protected void LlenaComboEmpresas()
        {
            try
            {
                this.StoreEmpresas.RemoveAll();

                this.StoreEmpresas.DataSource = DAOMoshi.ObtieneCatalogoEmpresas(this.Usuario);
                this.StoreEmpresas.DataBind();
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Convenios con Empresas", "Ocurrió un error al consultar las Empresas.").Show();
                DALLealtad.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Controla el evento de selección de un elemento del combo de Empresa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void select_Empresa(object sender, DirectEventArgs e)
        {
            try
            {
                Int32 IdTipoOperacion = int.Parse(this.cBoxTipoOperacion.SelectedItem.Value);

                DataSet dsParams = 
                    DAOMoshi.ObtieneParamsConvEmpresa(IdTipoOperacion, this.Usuario, 
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                Int32 IdRegla = Convert.ToInt32(dsParams.Tables[0].Rows[0]["IdRegla"].ToString());
                Int32 ID_CadenaComercial = Convert.ToInt32(dsParams.Tables[0].Rows[0]["IdCadenaComercial"].ToString());

                HttpContext.Current.Session.Add("ID_ReglaConvEmpr", IdRegla);
                HttpContext.Current.Session.Add("ID_CadenaMoshi", ID_CadenaComercial);

                LlenaGridPropiedades(IdTipoOperacion, ID_CadenaComercial, IdRegla);
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Convenios con Empresas", "Ocurrió un error al consultar los valores de la regla.").Show();
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Llena el PropertyGrid con los parámetros y valores de la regla
        /// </summary>
        /// <param name="ID_TipoOperacion">Identificador del tipo de operación</param>
        /// <param name="ID_CadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="ID_Regla">Identificador de la regla</param>
        protected void LlenaGridPropiedades(Int32 ID_TipoOperacion, Int32 ID_CadenaComercial, Int32 ID_Regla)
        {
            try
            {
                PropertyGridParameterCollection source = new PropertyGridParameterCollection();
                this.GridPropiedades.SetSource(source);

                foreach (ValorRegla unaProp in DAOMoshi.ListaValoresReglaConvEmpresa(
                    ID_TipoOperacion, ID_Regla, ID_CadenaComercial,
                    int.Parse(this.cBoxEmpresa.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Values)
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
                                ConfigurationManager.AppSettings["IDApplication"].ToString()), "ValorReglaDiasSemanaConvenio").Valor
                            ||
                            unaProp.Nombre == Configuracion.Get(Guid.Parse(
                                ConfigurationManager.AppSettings["IDApplication"].ToString()), "ValorReglaSucursalesConvenio").Valor)
                        {

                            MultiCombo mc = new MultiCombo() { ID = unaProp.Nombre.Replace('@', '_') };

                            if (unaProp.Nombre == Configuracion.Get(Guid.Parse(
                                ConfigurationManager.AppSettings["IDApplication"].ToString()), "ValorReglaDiasSemanaConvenio").Valor)
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

                            else
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
                                        TextField df = new TextField() { ID = unaProp.Nombre.Replace('@', '_') };
                                        df.Text = unaProp.Valor;
                                        df.AllowBlank = false;
                                        GridProp.Editor.Add(df); break;

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
                                        SpinnerField df = new SpinnerField() { ID = unaProp.Nombre.Replace('@', '_') };
                                        df.Text = unaProp.Valor;
                                        df.AllowBlank = false;
                                        GridProp.Editor.Add(df); break;
                                    }
                                case "MONEY":
                                    {
                                        TextField df = new TextField() { ID = unaProp.Nombre.Replace('@', '_') };

                                        df.Text = String.Format("{0:c}", float.Parse(unaProp.Valor));
                                        //df.MaskRe = "^[0-9]{1,8}(.[0-9]{0,2})?$";
                                        df.AllowBlank = false;
                                        GridProp.Editor.Add(df); break;
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
                            X.Msg.Notify("Convenios con Empresas", "<br />  <b> La Hora de Inicio debe ser un valor entre las 00:00 y las 23:59 horas</b> <br /> ").Show();
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
                            X.Msg.Notify("Convenios con Empresas", "<br />  <b> La Hora de Fin debe ser un valor entre las 00:00 y las 23:59 horas</b> <br /> ").Show();
                            return;
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
                    X.Msg.Notify("Convenios con Empresas", "Modificación de Valores de la Regla <br /><br />  <b> La Fecha Inicial debe ser menor o igual que la Fecha Final </b> <br />  <br /> ").Show();
                    return;
                }

                if (DateTime.Compare(laFechaFinal, DateTime.Now) == 0)
                {
                    X.Msg.Notify("Convenios con Empresas", "Modificación de Valores de la Regla <br /><br />  <b> La Fecha Final no puede ser igual a la actual </b> <br />  <br /> ").Show();
                    return;
                }

                //Guardar Valores
                LNMoshi.ModificaValoresReglaPorGrupoMA(losCambios, Int64.Parse(HttpContext.Current.Session["ID_CadenaMoshi"].ToString()),
                    Int64.Parse(HttpContext.Current.Session["ID_ReglaConvEmpr"].ToString()),
                    Convert.ToInt32(this.cBoxEmpresa.SelectedItem.Value),
                    this.Usuario);

                LlenaGridPropiedades(Int32.Parse(this.cBoxTipoOperacion.SelectedItem.Value),
                    Int32.Parse(HttpContext.Current.Session["ID_CadenaMoshi"].ToString()),
                    Int32.Parse(HttpContext.Current.Session["ID_ReglaConvEmpr"].ToString()));

                X.Msg.Notify("Convenios con Empresas", "Modificación de Valores de la Regla <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Notify("Convenios con Empresas", "Modificación de Valores de la Regla <br /><br />  <b> D E C L I N A D O </b> <br />  <br /> ").Show();
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
                LlenaGridPropiedades(Int32.Parse(this.cBoxTipoOperacion.SelectedItem.Value),
                    Int32.Parse(HttpContext.Current.Session["ID_CadenaMoshi"].ToString()),
                    Int32.Parse(HttpContext.Current.Session["ID_ReglaConvEmpr"].ToString()));
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                return;
            }
        }
    }
}