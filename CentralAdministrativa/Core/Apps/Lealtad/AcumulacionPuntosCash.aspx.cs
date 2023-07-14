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
using System.Web;

namespace Lealtad
{
    public partial class AcumulacionPuntosCash : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Acumulación de Puntos CASHAE
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    hdnIdCadenaCash.Value = DAOReglasLealtad.ObtieneIDCadenaComercial(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveColectivaCadena").Valor,
                        this.Usuario);

                    LlenaComboNivelesLealtad();
                    LlenaComboReglas();
                }

                if (!X.IsAjaxRequest)
                {
                    
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
                DataTable dtNiveles = DAOReglasLealtad.ObtieneCatalogoNivelesLealtad(this.Usuario);

                StoreNivelLealtad.DataSource = dtNiveles;
                StoreNivelLealtad.DataBind();

                if (dtNiveles.Rows.Count == 1)
                {
                    cBoxNivelLealtad.SelectedIndex = 0;
                }
            }

            catch (CAppException caEx)
            {
                DALLealtad.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Niveles de Lealtad", caEx.Message).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Niveles de Lealtad", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el combo de reglas a partir del catálogo de base de datos
        /// </summary>
        protected void LlenaComboReglas()
        {
            try
            {
                StoreRegla.DataSource = DAOReglasLealtad.ObtieneReglasAcumulacion(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                StoreRegla.DataBind();
            }

            catch (CAppException caEx)
            {
                DALLealtad.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Reglas", caEx.Message).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Reglas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento de selección de un elemento del combo de reglas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void selectRegla_Click(object sender, DirectEventArgs e)
        {
            try
            {
                LlenaGridPropiedades();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Acumulación de Puntos", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el PropertyGrid con los parámetros y valores de la regla
        /// </summary>
        protected void LlenaGridPropiedades()
        {
            try
            {
                Int64 ID_CadenaComercial = Convert.ToInt64(hdnIdCadenaCash.Value);
                Int64 ID_Regla = Convert.ToInt64(cBoxReglas.SelectedItem.Value);

                PropertyGridParameterCollection source = new PropertyGridParameterCollection();
                this.GridPropiedades.SetSource(source);

                foreach (ValorRegla unaProp in DAOReglasLealtad.ListaValoresReglaPorGpoCta(ID_Regla, 
                    ID_CadenaComercial, int.Parse(this.cBoxNivelLealtad.SelectedItem.Value), 
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Values)
                {
                    PropertyGridParameter GridProp = new PropertyGridParameter(unaProp.Descripcion, unaProp.Valor);

                    Dictionary<String, ValorRegla> laRespuesta = DAOReglasLealtad.ListaDeValoresReglaPredefinidos(ID_Regla, ID_CadenaComercial, unaProp.ID_ValorRegla, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));


                    if (laRespuesta.Count >= 1)
                    {
                        ComboBox comboBox = new ComboBox { ID = unaProp.Nombre.Replace('@', '_'), Editable = false, EmptyText = "Selecciona una Opción...", AllowBlank = false };

                        foreach (ValorRegla unValorEstablecido in laRespuesta.Values)
                        {
                            ListItem unItem = new ListItem(unValorEstablecido.Descripcion, unValorEstablecido.Valor);
                            comboBox.Items.Add(unItem);
                        }

                        PropertyGridParameter GridProp2 = new PropertyGridParameter(unaProp.Descripcion, "(Selecciona un Valor)");
                        GridProp2.DisplayName = unaProp.Descripcion;
                        GridProp2.Editor.Add(comboBox);

                        this.GridPropiedades.AddProperty(GridProp2);
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
                    if (param.Name.ToUpper().Equals("@FechaInicio".ToUpper()))
                    {
                        try
                        {
                            laFechaInicial = DateTime.Parse(param.Value.ToString());
                        }
                        catch (Exception)
                        {
                        }
                    }

                    if (param.Name.ToUpper().Equals("@FechaFin".ToUpper()))
                    {
                        try
                        {
                            laFechaFinal = DateTime.Parse(param.Value.ToString());
                        }
                        catch (Exception)
                        {
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
                    X.Msg.Notify("Acumulación de Puntos", "Modificación de Valores de la Regla <br /><br />  <b> La Fecha Inicial debe ser menor o igual que la Fecha Final </b> <br />  <br /> ").Show();
                    return;
                }

                if (DateTime.Compare(laFechaFinal, DateTime.Now) == 0)
                {
                    X.Msg.Notify("Acumulación de Puntos", "Modificación de Valores de la Regla <br /><br />  <b> La Fecha Final no puede ser igual a la actual </b> <br />  <br /> ").Show();
                    return;
                }

                //Guardar Valores
                LNReglasLealtad.ModificaValoresReglaPorGpoCuenta(losCambios, Convert.ToInt64(hdnIdCadenaCash.Value),
                    Convert.ToInt64(cBoxReglas.SelectedItem.Value), 
                    Convert.ToInt32(this.cBoxNivelLealtad.SelectedItem.Value),
                    this.Usuario);

                LlenaGridPropiedades();

                X.Msg.Notify("Acumulación de Puntos", "Modificación de Valores de la Regla <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Notify("Acumulación de Puntos", "Modificación de Valores de la Regla <br /><br />  <b> D E C L I N A D O </b> <br />  <br /> ").Show();
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
                LlenaGridPropiedades();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                return;
            }
        }
    }
}