using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.LogicaNegocio;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

namespace Lealtad
{
    public partial class Regla008 : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!IsPostBack)
                {
                    //Valida que el Usuario tenga permisos de Ver la pagina
                    PreparaGridCadenasComerciales();

                    Regla laREgla = DAORegla.ObtnenRegla(Int64.Parse(ID_REGLA.Text));

                    lblDescripcionRegla.Text = laREgla.Descripcion;
                    lblNombreRegla.Text = laREgla.Nombre;

                }

                if (!X.IsAjaxRequest)
                {
                    this.BindDataCadenasComerciales();
                    //this.BindDataSaldos(0,"");
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }


        private void AgregaRecordFilesCadenasComerciales()
        {
            Store2.AddField(new RecordField("ID_Colectiva"));
            Store2.AddField(new RecordField("ClaveColectiva"));
            Store2.AddField(new RecordField("NombreORazonSocial"));
            Store2.AddField(new RecordField("APaterno"));
            Store2.AddField(new RecordField("AMaterno"));
            Store2.AddField(new RecordField("RFC"));
            Store2.AddField(new RecordField("Email"));
            Store2.AddField(new RecordField("Estatus"));
            Store2.AddField(new RecordField("Clave"));
            Store2.AddField(new RecordField("Descripcion"));
            Store2.AddField(new RecordField("ID_CadenaComercial"));
            Store2.AddField(new RecordField("ClaveSucursalAdmin"));
            Store2.AddField(new RecordField("AfiliacionAdmin"));


        }


        protected void PreparaGridCadenasComerciales()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFilesCadenasComerciales();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colID_Colectiva = new Column();
            colID_Colectiva.DataIndex = "ID_Colectiva";
            colID_Colectiva.Header = "ID";
            colID_Colectiva.Sortable = true;
            colID_Colectiva.Hidden = true;

            lasColumnas.Columns.Add(colID_Colectiva);

            Column colClaveColectiva = new Column();
            colClaveColectiva.DataIndex = "Estatus";
            colClaveColectiva.Header = "Estatus ";
            colClaveColectiva.Sortable = true;
            lasColumnas.Columns.Add(colClaveColectiva);

            Column colNombreORazonSocial = new Column();
            colNombreORazonSocial.DataIndex = "NombreORazonSocial";
            colNombreORazonSocial.Header = "Nombre";
            colNombreORazonSocial.Sortable = true;
            lasColumnas.Columns.Add(colNombreORazonSocial);

            Column colAPaterno = new Column();
            colAPaterno.DataIndex = "APaterno";
            colAPaterno.Header = "Ubicación 1";
            colAPaterno.Sortable = true;
            lasColumnas.Columns.Add(colAPaterno);

            Column colAMaterno = new Column();
            colAMaterno.DataIndex = "AMaterno";
            colAMaterno.Header = "Ubicación 2";
            colAMaterno.Sortable = true;
            lasColumnas.Columns.Add(colAMaterno);

            Column colRFC = new Column();
            colRFC.DataIndex = "RFC";
            colRFC.Header = "RFC";
            colRFC.Sortable = true;
            lasColumnas.Columns.Add(colRFC);

            Column colID_CadenaComercial = new Column();
            colID_CadenaComercial.DataIndex = "ID_CadenaComercial";
            colID_CadenaComercial.Header = "ID_CadenaComercial";
            colID_CadenaComercial.Sortable = true;
            colID_CadenaComercial.Hidden = true;
            lasColumnas.Columns.Add(colID_CadenaComercial);

            Column colClaveSucursalAdmin = new Column();
            colClaveSucursalAdmin.DataIndex = "ClaveSucursalAdmin";
            colClaveSucursalAdmin.Header = "ClaveSucursalAdmin";
            colClaveSucursalAdmin.Sortable = true;
            colClaveSucursalAdmin.Hidden = true;
            lasColumnas.Columns.Add(colClaveSucursalAdmin);

            //Column colAfiliacionAdmin = new Column();
            //colAfiliacionAdmin.DataIndex = "AfiliacionAdmin";
            //colAfiliacionAdmin.Header = "AfiliacionAdmin";
            //colAfiliacionAdmin.Sortable = true;
            //lasColumnas.Columns.Add(colAfiliacionAdmin);

            Column colEmail = new Column();
            colEmail.DataIndex = "Email";
            colEmail.Header = "Email";
            colEmail.Sortable = true;
            lasColumnas.Columns.Add(colEmail);

            //ImageCommandColumn acciones = new ImageCommandColumn();
            //acciones.Header = "Acciones";
            //acciones.Width = 55;
            //ImageCommand edit = new ImageCommand();
            //edit.Icon = Icon.NoteEdit;
            //edit.CommandName = "Estatus";
            //edit.ToolTip.Text = "Consultar Saldo";
            //acciones.Commands.Add(edit);



            //AGREGAR COLUMNAS
            //GridPanel1.ColumnModel.Columns.Add(acciones);
            ////GridPanel2.ColumnModel.Columns.Add(colID_Colectiva);

            ////GridPanel2.ColumnModel.Columns.Add(colClaveColectiva);
            ////GridPanel2.ColumnModel.Columns.Add(colNombreORazonSocial);
            ////GridPanel2.ColumnModel.Columns.Add(colAPaterno);
            ////GridPanel2.ColumnModel.Columns.Add(colAMaterno);
            ////GridPanel2.ColumnModel.Columns.Add(colRFC);
            ////GridPanel2.ColumnModel.Columns.Add(colEmail);
            ////GridPanel2.ColumnModel.Columns.Add(colID_CadenaComercial);
            ////GridPanel2.ColumnModel.Columns.Add(colClaveSucursalAdmin);
            // GridPanel2.ColumnModel.Columns.Add(colAfiliacionAdmin);


            //AGREGAR EVENTOS
            /////GridPanel2.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_CadenaComercial", "this.getRowsValues({ selectedOnly: true })[0].ID_CadenaComercial", ParameterMode.Raw));

            //GridPanel2.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("Afiliacion", "this.getRowsValues({ selectedOnly: true })[0].AfiliacionAdmin", ParameterMode.Raw));





        }


        protected void RefreshGridSaldos(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                Int64 idCol = Int64.Parse(HttpContext.Current.Session["ID_ColectivaSaldo"].ToString());
                //string afil = HttpContext.Current.Session["AfiliacionSaldo"].ToString();

            }
            catch (Exception)
            {
            }

        }

        protected void RefreshGridCadenasComerciales(object sender, StoreRefreshDataEventArgs e)
        {
            PreparaGridCadenasComerciales();
            this.BindDataCadenasComerciales();

        }



        private void BindDataCadenasComerciales()
        {


            Guid IDApp = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());

            //GridPanel2.GetStore().DataSource = DAOColectiva.ListaSaldosColectivaIDTipoColectiva(-1, Configuracion.Get(IDApp, "ClaveTipoColectivaEmpleado").Valor);
            //GridPanel2.GetStore().DataBind();
            cmbCadenaComercial.GetStore().DataSource = DAOCatalogos.ListaEmpleadosMiembrosClubGrupoComercial(-1, Configuracion.Get(IDApp, "ClaveTipoColectivaCadenaComercial").Valor, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            cmbCadenaComercial.GetStore().DataBind();
        }

        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {

            //  Panel5.Collapsed = true;

        }

        protected void GridEmpleados_DblClik(object sender, DirectEventArgs e)
        {
            try
            {

                String unvalor = cmbCadenaComercial.Value.ToString();
                Int64 ID_Regla = Int64.Parse(ID_REGLA.Text);
                //String unaAfil = e.ExtraParams["Afiliacion"];

                //Panel5.Title = "Configuraciones de la Regla";

                HttpContext.Current.Session.Add("ID_Colectiva", unvalor);
                HttpContext.Current.Session.Add("ID_Regla", ID_Regla.ToString());
                //HttpContext.Current.Session.Add("AfiliacionSaldo", unaAfil);

                Int64 ID_CadenaComercial = Int64.Parse(unvalor);


                PreparaGripPropiedades(ID_CadenaComercial, ID_Regla);
            }
            catch (Exception)
            {
            }

            //FichaDeposito nuevaFicha = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, new IUsuario());

            //DAOFichaDeposito.ConsultaFichaDeposito(e.
            ////Panel5.Collapsed = false;
        }


        protected void PreparaGripPropiedades(Int64 ID_CadenaComercial, Int64 ID_Regla)
        {
            try
            {
                //int e = 0;

                PropertyGridParameterCollection source = new PropertyGridParameterCollection();
                // populating
                this.GridPropiedades.SetSource(source);


                foreach (ValorRegla unaProp in DAORegla.ListaDeValoresRegla(ID_Regla, ID_CadenaComercial, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Values)
                {
                    PropertyGridParameter GridProp = new PropertyGridParameter(unaProp.Descripcion, unaProp.Valor);

                    Dictionary<String, ValorRegla> laRespuesta = DAORegla.ListaDeValoresReglaPredefinidos(ID_Regla, ID_CadenaComercial, unaProp.ID_ValorRegla, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));


                    if (laRespuesta.Count >= 1)
                    {
                        // ComboBox tf = new ComboBox() { ID = unaProp.Nombre.Replace('@','_') };
                        ComboBox comboBox = new ComboBox { ID = unaProp.Nombre.Replace('@', '_'), Editable = false, EmptyText = "Selecciona una Opción...", AllowBlank = false };

                        foreach (ValorRegla unValorEstablecido in laRespuesta.Values)
                        {
                            //tf.AddItem(, );
                            Ext.Net.ListItem unItem = new Ext.Net.ListItem(unValorEstablecido.Descripcion, unValorEstablecido.Valor);
                            comboBox.Items.Add(unItem);
                        }
                        //  tf.StoreID = "Store1";

                        //tf.GetStore().DataSource = laRespuesta.Values;
                        //tf.GetStore().DataBind();
                        //tf.SetValue(unaProp.Valor);
                        //tf.AllowBlank = false;
                        //tf.Render();
                        //GridProp.Editor.Add(tf);


                        //comboBox.GetStore().DataSource = LNColectiva.ObtieneColectivasPorIDTipoColectiva(unaProp.ID_TipoColectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                        //comboBox.GetStore().DataBind();



                        PropertyGridParameter GridProp2 = new PropertyGridParameter(unaProp.Descripcion, "(Selecciona un Valor)");
                        GridProp2.DisplayName = unaProp.Descripcion;



                        GridProp2.Editor.Add(comboBox);
                        GridPropiedades.AddProperty(GridProp2);

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
                                    // df.Render();
                                    // df.Listeners.Disable.Handler = "this.setValue(Ext.util.Format.number(newValue.replace([,]g, \"\"), \"$0,0.00\"));";
                                    //df.Listeners.Change.Handler = "this.setValue(Ext.util.Format.number(newValue.replace([,]g, \"\"), \"$0,0.00\"));";
                                    GridProp.Editor.Add(df); break;

                                }
                        }
                    }

                    GridProp.DisplayName = unaProp.Descripcion;

                    GridPropiedades.AddProperty(GridProp);

                }

                this.GridPropiedades.Render();



            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }



        protected void Button1_Click(object sender, DirectEventArgs e)
        {
            try
            {
                DateTime laFechaInicial = DateTime.Now;
                DateTime laFechaFinal = DateTime.Now;

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

                if (laFechaFinal < laFechaInicial)
                {
                    X.Msg.Notify("Configuración", "Modificación de Valores de la Regla <br /><br />  <b> Fecha Inicial Mayor que la Fecha Final </b> <br />  <br /> ").Show();
                    return;
                }

                if (laFechaFinal < DateTime.Now)
                {
                    X.Msg.Notify("Configuración", "Modificación de Valores de la Regla <br /><br />  <b> La Fecha Final no puede ser igual a la actual </b> <br />  <br /> ").Show();
                    return;
                }

                //Guardar Valores

                LNRegla.ModificaValoresRegla(losCambios, Int64.Parse(HttpContext.Current.Session["ID_Colectiva"].ToString()), Int64.Parse(ID_REGLA.Text), this.Usuario);

                PreparaGripPropiedades(Int64.Parse(HttpContext.Current.Session["ID_Colectiva"].ToString()), Int64.Parse(ID_REGLA.Text));

                X.Msg.Notify("Configuración", "Modificación de Valores de la Regla <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();

            }
            catch (Exception)
            {
                X.Msg.Notify("Configuración", "Modificación de Valores de la Regla <br /><br />  <b> D E C L I N A D O </b> <br />  <br /> ").Show();
            }

        }

    }
}