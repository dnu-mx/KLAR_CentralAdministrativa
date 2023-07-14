using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using Ext.Net;
using DALCentralAplicaciones.LogicaNegocio;
using DALAutorizador.LogicaNegocio;
using System.Web.Security;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using Interfases;
using System.Configuration;
using DALCentralAplicaciones.Utilidades;
using Framework;
using Log_PCI.Entidades;

namespace TpvWeb
{
    public partial class ConfiguracionWebCadenasComercial : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!IsPostBack)
                {
                    //Valida que el Usuario tenga permisos de Ver la pagina
                    PreparaGridEmpleados();

                }

                if (!X.IsAjaxRequest)
                {
                    this.BindDataEmpleados();
                    //this.BindDataSaldos(0,"");
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }


        private void AgregaRecordFilesEmpleados()
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

        [DirectMethod(Namespace = "TpvWeb")]
        public void AfterEdit(int ID_Cuenta, string newValue, Int64 ID_Colectiva, string TipoCuentaISO, string codigoMoneda, bool AceptaEditar, string afiliacion)
        {
            RespuestaTransaccional resp = new RespuestaTransaccional();

            Guid AppId = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());

            try
            {
                LogHeader logTEMP = new LogHeader();
                resp = LNSaldos.ModificarSaldo(float.Parse(newValue), ID_Cuenta, ID_Colectiva, TipoCuentaISO, codigoMoneda, AceptaEditar, AppId, this.Usuario, afiliacion, logTEMP);

                // Send Message...
                if (resp.IsAutorizada())
                {
                    X.Msg.Notify("Respuesta Transaccional", "Resultados de la Operación <br />  Codigo Respuesta: <b>" + resp.CodigoRespuesta + "</b> <br /> Numero Autorización: <b>" + resp.Autorizacion + "</b> <br />").Show();
                    X.Msg.Notify("Respuesta Transaccional", "Modificacion de Saldo <br />  <br /> <b> A U T O R I Z A D A </b> <br />  <br /> ").Show();

                }
                else
                {
                    if (resp.IsTimeOut)
                    {
                        X.Msg.Notify("Respuesta Transaccional", "Mensaje: <b>" + resp.ResultadoOperacion + "</b> <br />").Show();
                        X.Msg.Notify("Respuesta Transaccional", "Modificacion de Saldo <br /><br />  <b> T I M E    O U T </b> <br />  <br /> ").Show();
                    }
                    else
                    {
                        if ((resp.DescripcionRespuesta == null) || (resp.DescripcionRespuesta == ""))
                        {
                            X.Msg.Notify("Respuesta Transaccional", "Mensaje: <b>" + resp.ResultadoOperacion + "</b> <br />").Show();
                            X.Msg.Notify("Respuesta Transaccional", "Modificacion de Saldo <br /><br />  <b> D E C L I N A D A </b> <br />  <br /> ").Show();
                        }
                        else
                        {
                            X.Msg.Notify("Respuesta Transaccional", "<br />  Codigo Respuesta: <b>" + resp.CodigoRespuesta + "</b> <br /> Numero Autorización: <b>" + resp.Autorizacion + "</b> <br />" + "Mensaje: <b>" + resp.DescripcionRespuesta + "</b> <br />").Show();
                            X.Msg.Notify("Respuesta Transaccional", "Modificacion de Saldo <br /><br />  <b> D E C L I N A D A </b> <br />  <br /> ").Show();
                        }
                    }
                }

            }
            catch (CAppException err)
            {
                X.Msg.Alert("Modificaciones de Saldos", err.Mensaje()).Show();
            }
            catch (Exception )
            {
                X.Msg.Alert("Modificaciones de Saldos", "No es Posible Modificar el Saldo.").Show();
            }
            finally
            {
            }


            //this.GridPanel1.Store.Primary.CommitChanges();
        }

        protected void PreparaGridEmpleados()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFilesEmpleados();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colID_Colectiva = new Column();
            colID_Colectiva.DataIndex = "ID_Colectiva";
            colID_Colectiva.Header = "ID";
            colID_Colectiva.Sortable = true;
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
            GridPanel2.ColumnModel.Columns.Add(colID_Colectiva);

            GridPanel2.ColumnModel.Columns.Add(colClaveColectiva);
            GridPanel2.ColumnModel.Columns.Add(colNombreORazonSocial);
            GridPanel2.ColumnModel.Columns.Add(colAPaterno);
            GridPanel2.ColumnModel.Columns.Add(colAMaterno);
            GridPanel2.ColumnModel.Columns.Add(colRFC);
            GridPanel2.ColumnModel.Columns.Add(colEmail);
            GridPanel2.ColumnModel.Columns.Add(colID_CadenaComercial);
            GridPanel2.ColumnModel.Columns.Add(colClaveSucursalAdmin);
            // GridPanel2.ColumnModel.Columns.Add(colAfiliacionAdmin);


            //AGREGAR EVENTOS
            GridPanel2.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Colectiva", "this.getRowsValues({ selectedOnly: true })[0].ID_Colectiva", ParameterMode.Raw));
            GridPanel2.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("Afiliacion", "this.getRowsValues({ selectedOnly: true })[0].AfiliacionAdmin", ParameterMode.Raw));





        }


        protected void RefreshGridSaldos(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                Int64 idCol = Int64.Parse(HttpContext.Current.Session["ID_ColectivaSaldo"].ToString());
                //string afil = HttpContext.Current.Session["AfiliacionSaldo"].ToString();

            }
            catch (Exception )
            {
            }

        }

        protected void RefreshGridEmpleados(object sender, StoreRefreshDataEventArgs e)
        {
            PreparaGridEmpleados();
            this.BindDataEmpleados();

        }



        private void BindDataEmpleados()
        {


            Guid IDApp = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());

            //GridPanel2.GetStore().DataSource = DAOColectiva.ListaSaldosColectivaIDTipoColectiva(-1, Configuracion.Get(IDApp, "ClaveTipoColectivaEmpleado").Valor);
            //GridPanel2.GetStore().DataBind();
            LogHeader logTEMP = new LogHeader();
            GridPanel2.GetStore().DataSource = DAOCatalogos.ListaEmpleadosMiembrosClubGrupoComercial(-1, Configuracion.Get(IDApp, "ClaveTipoColectivaCadenaComercial").Valor, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), logTEMP);
            GridPanel2.GetStore().DataBind();
        }

        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {

            Panel5.Collapsed = true;

        }

        protected void GridEmpleados_DblClik(object sender, DirectEventArgs e)
        {
            try
            {

                String unvalor = e.ExtraParams["ID_Colectiva"];
                //String unaAfil = e.ExtraParams["Afiliacion"];

                Panel5.Title = "Configuraciones de la Cadena Comercial No. [" + unvalor + "]";

                HttpContext.Current.Session.Add("ID_Colectiva", unvalor);
                //HttpContext.Current.Session.Add("AfiliacionSaldo", unaAfil);

                Int64 ID_colectiva = Int64.Parse(unvalor);


                PreparaGripPropiedades(ID_colectiva);
            }
            catch (Exception )
            {
            }

            //FichaDeposito nuevaFicha = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, new IUsuario());

            //DAOFichaDeposito.ConsultaFichaDeposito(e.
            Panel5.Collapsed = false;
        }


        protected void PreparaGripPropiedades(Int64 ID_CadenaComercial)
        {
            try
            {
                PropertyGridParameterCollection source = new PropertyGridParameterCollection();
                // populating
                this.GridPropiedades.SetSource(source);


                foreach (DALPuntoVentaWeb.Entidades.Propiedad unaProp in DALPuntoVentaWeb.LogicaNegocio.LNPropiedad.ObtieneParametros(ID_CadenaComercial, ContextoInicial.ContextoServicio.ConnectionString))
                {
                    PropertyGridParameter GridProp = new PropertyGridParameter(unaProp.Nombre, unaProp.Valor);
                    GridProp.DisplayName = unaProp.Descripcion;

                    GridPropiedades.AddProperty(GridProp);

                }

            }
            catch (Exception)
            {
            }
        }



        protected void Button1_Click(object sender, DirectEventArgs e)
        {
            try
            {
                List<DALPuntoVentaWeb.Entidades.Propiedad> losCambios = new List<DALPuntoVentaWeb.Entidades.Propiedad>();


                //Obtiene las propiedades que cambiaron
                foreach (PropertyGridParameter param in this.GridPropiedades.Source)
                {
                    if (param.IsChanged)
                    {
                        DALPuntoVentaWeb.Entidades.Propiedad unaProp = new DALPuntoVentaWeb.Entidades.Propiedad(param.Name, param.Value.ToString(), "", 1);
                        losCambios.Add(unaProp);
                    }
                }

                //Guardar Valores
                DALPuntoVentaWeb.LogicaNegocio.LNPropiedad.ModificaParametros(losCambios, Int64.Parse(HttpContext.Current.Session["ID_Colectiva"].ToString()), ContextoInicial.ContextoServicio.ConnectionString, this.Usuario);

                // DALCentralAplicaciones.LogicaNegocio.ValoresInicial.InicializarContexto();

                PreparaGripPropiedades(Int64.Parse(HttpContext.Current.Session["ID_Colectiva"].ToString()));

                X.Msg.Notify("Configuración", "Modificación de Configuración <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();


            }
            catch (Exception)
            {
                X.Msg.Notify("Configuración", "Modificación de Configuración <br /><br />  <b> D E C L I N A D O </b> <br />  <br /> ").Show();
            }

        }

    }
}