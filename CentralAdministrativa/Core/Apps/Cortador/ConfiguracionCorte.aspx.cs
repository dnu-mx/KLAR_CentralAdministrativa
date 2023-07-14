using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DALCentralAplicaciones.LogicaNegocio;
using System.Web.Security;
using DALCentralAplicaciones.Entidades;
using Ext.Net;
using DALEventos.Utilidades;
using DALAutorizador.Entidades;
using DALAutorizador.BaseDatos;
using System.Configuration;
using Interfases.Exceptiones;

namespace Cortador
{
    public partial class ConfiguracionCorte : DALCentralAplicaciones.PaginaBaseCAPP
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                  

                    //LlenaTiposColectiva();
                }

                if (!IsPostBack)
                {
                    PreparaGridConfig();
                    PreparaGridScript();
                    BindDataConfiguracion();
                    //BindDataScript();

                    //llena los combos
                    LlenaPeriodo();
                    LlenaTipoCuenta();
                    LlenaTipoContrato();
                    LlenaEvento();
                    LlenaTipoColectiva();
                }

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        public void LlenaPeriodo()
        {
            StorePeriodo.DataSource = DAOCatalogos.ListaPeriodo(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            StorePeriodo.DataBind();
        }
        public void LlenaTipoCuenta()
        {

            StoreTipoCta.DataSource = DAOCuenta.ObtieneTiposCuentaEjecutor(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            StoreTipoCta.DataBind();

        }
        public void LlenaTipoContrato()
        {
            
            StoreTipoContrato.DataSource = DAOCatalogos.ListaTipoContrato(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            StoreTipoContrato.DataBind();
        }
        public void LlenaEvento()
        {

            StoreEvento.DataSource = DAOCatalogos.ListaEventosNoTransaccionales(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            StoreEvento.DataBind();
        }

        public void LlenaTipoColectiva()
        {

            StoreTipoColectiva.DataSource = DAOCatalogos.ListaTipoColectivaParaEjecutor(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            StoreTipoColectiva.DataBind();
        }

         private void BindDataConfiguracion()
        {
            GridPanel1.GetStore().DataSource = DAOCortes.ObtieneConfiguracion(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel1.GetStore().DataBind();
        }

        private void BindDataScript(Int32 ID_Evento)
        {
            GridPanel2.GetStore().DataSource = DAOCortes.ObtieneScriptsContables(ID_Evento,this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel2.GetStore().DataBind();
        }


        protected void btnGuardarConfig_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {

                  //Guarda
                    DAOCortes.AgregarConfiguracion(NuevaConf(), this.Usuario);

                    //Limpia el Formulario 
                    FormPanel1.Reset();

                    //actualiza el Store del Grid
                    PreparaGridConfig();
                    BindDataConfiguracion();
                }
                catch
                {
                    //Msg.Text = "An exception occurred creating the user.";
                    X.Msg.Alert("Agregar Configuracion", "Ocurrio Un Error al crear la configuracion, Intentalo más tarde").Show();
                    return;
                }

            }
            catch (Exception )
            {
                X.Msg.Alert("Status", "Ocurrio Un Error al crear la Configuracion, Intentalo más tarde").Show();
                return;
            }

        }

        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {

            PanelScript.Collapsed = true;
        }

        protected void GridConfig_DblClik(object sender, DirectEventArgs e)
        {
            try
            {

                Int32 IdTipocuenta = Int32.Parse(e.ExtraParams["ID_TipoCuenta"] == null ? "0" : e.ExtraParams["ID_TipoCuenta"]);
                Int32 ID_TipoContrato = Int32.Parse(e.ExtraParams["ID_TipoContrato"] == null ? "0" : e.ExtraParams["ID_TipoContrato"]);
                Int32 IDEvento = Int32.Parse(e.ExtraParams["ID_Evento"] == null ? "0" : e.ExtraParams["ID_Evento"]);
                String DesEvento = e.ExtraParams["DesEvento"] == null ? "" : e.ExtraParams["DesEvento"];
                HttpContext.Current.Session.Add("ID_Evento", IDEvento);

                PanelScript.Title = "Scripts de Ejecucion Periódica [" +DesEvento+"]";
                PanelScript.Collapsed = false;

                //PanelScript.Title = "Scripts de Ejecucion Periódica [" + DesEvento + "]";
                //PanelScript.Collapsed = false;

                //String Tooltip = DAOCatalogos.ParametrosContrato(ID_TipoContrato);
               
                
                PreparaGridScript();
                BindDataScript(IDEvento);

                cmbTipoCuenta2.Value = IdTipocuenta;
                PanelScript.Collapsed = false;


            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                X.Msg.Alert("Mostrar Script", "Error al Mostrar el Script Contable").Show();
                return;
            }

        }

        protected void btnCancelarConfig_Click(object sender, EventArgs e)
        {
            FormPanel1.Reset();
           // FormPanel1.Collapsed = true;
        }

        protected void btnGuardarScript_Click(object sender, EventArgs e)
        {
            try
            {
                
                //Agrega el Script al Evento
                DAOCortes.AgregarScript(NuevoScript(), this.Usuario);

                //  //Limpia el Formulario 
                FormPanel2.Reset();

                //actualiza el Store del Grid
                PreparaGridScript();
                BindDataScript((Int32)HttpContext.Current.Session.Contents["ID_Evento"]);


            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                X.Msg.Alert("Guardar Script", "Error al Guardar el Script Contable").Show();
                return;
            }

        }

        protected void btnVer_Parametros(object sender, EventArgs e)
        {
            try
            {

                Window1.Html = DAOCatalogos.ParametrosContrato(0);
                this.Window1.Show();

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                X.Msg.Alert("Guardar Script", "Error al Guardar el Script Contable").Show();
                return;
            }
        }


        protected void btnCancelarScript_Click(object sender, EventArgs e)
        {
            FormPanel2.Reset();
           // FormPanel2.Collapsed = true;
            PanelScript.Collapsed = true;
        }

        protected void PreparaGridConfig()
        {

            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Acciones";
            acciones.Width = 55;
            //acciones.PrepareToolbar.Fn = "prepareToolbar";


            GridCommand elimina = new GridCommand();
            elimina.Icon = Icon.Delete;
            elimina.CommandName = "Eliminar";
            elimina.ToolTip.Text = "Eliminar Operacion Recursiva";
            acciones.Commands.Add(elimina);


            Column colID = new Column();
            colID.DataIndex = "ID_ConfiguracionCorte";
            colID.Header = "ID";
            colID.Sortable = true;
            colID.Hidden = true;
            lasColumnas.Columns.Add(colID);

            Column colClaveConfiguracion = new Column();
            colClaveConfiguracion.DataIndex = "ClaveConfiguracion";
            colClaveConfiguracion.Header = "Clave";
            colClaveConfiguracion.Width = 100;
            colClaveConfiguracion.Sortable = true;
            lasColumnas.Columns.Add(colClaveConfiguracion);

            Column colNombreConfiguracion = new Column();
            colNombreConfiguracion.DataIndex = "NombreConfiguracion";
            colNombreConfiguracion.Header = "Nombre";
            colNombreConfiguracion.Width = 150;
            colNombreConfiguracion.Sortable = true;
            lasColumnas.Columns.Add(colNombreConfiguracion);

            Column coldescConfiguracion = new Column();
            coldescConfiguracion.DataIndex = "descConfiguracion";
            coldescConfiguracion.Header = "Descripcion";
            coldescConfiguracion.Width = 200;
            coldescConfiguracion.Sortable = true;
            lasColumnas.Columns.Add(coldescConfiguracion);

            Column colEstatus = new Column();
            colEstatus.DataIndex = "Estatus";
            colEstatus.Header = "Estatus";
            colEstatus.Sortable = true;
            lasColumnas.Columns.Add(colEstatus);


            Column colDescTipoCuenta = new Column();
            colDescTipoCuenta.DataIndex = "DescTipoCuenta";
            colDescTipoCuenta.Header = "Tipo Cuenta";
            colDescTipoCuenta.Width = 200;
            colDescTipoCuenta.Sortable = true;
            lasColumnas.Columns.Add(colDescTipoCuenta);

            Column colDescPeriodo = new Column();
            colDescPeriodo.DataIndex = "DescPeriodo";
            colDescPeriodo.Header = "Periodo";
            colDescPeriodo.Width = 200;
            colDescPeriodo.Sortable = true;
            lasColumnas.Columns.Add(colDescPeriodo);

            Column colDescEvento = new Column();
            colDescEvento.DataIndex = "DescEvento";
            colDescEvento.Header = "Evento";
            colDescEvento.Width = 200;
            colDescEvento.Sortable = true;
            lasColumnas.Columns.Add(colDescEvento);

            Column coldescTipoContrato = new Column();
            coldescTipoContrato.DataIndex = "descTipoContrato";
            coldescTipoContrato.Header = "Contrato";
            coldescTipoContrato.Width = 200;
            coldescTipoContrato.Sortable = true;
            lasColumnas.Columns.Add(coldescTipoContrato);

            Column coldescTipoCuenta = new Column();
            coldescTipoCuenta.DataIndex = "ID_TipoCuenta";
            coldescTipoCuenta.Header = "Contrato";
            coldescTipoCuenta.Width = 200;
            coldescTipoCuenta.Sortable = true;
            coldescTipoCuenta.Hidden = true;
            lasColumnas.Columns.Add(coldescTipoCuenta);

            Column colID_Evento = new Column();
            colID_Evento.DataIndex = "ID_Evento";
            colID_Evento.Header = "Contrato";
            colID_Evento.Width = 200;
            colID_Evento.Sortable = true;
            colID_Evento.Hidden = true;
            lasColumnas.Columns.Add(colID_Evento);


            GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel1.ColumnModel.Columns.Add(colID);
            GridPanel1.ColumnModel.Columns.Add(colEstatus);
            GridPanel1.ColumnModel.Columns.Add(colDescPeriodo);
            GridPanel1.ColumnModel.Columns.Add(colNombreConfiguracion);
            GridPanel1.ColumnModel.Columns.Add(coldescConfiguracion);
            GridPanel1.ColumnModel.Columns.Add(colDescEvento);
            GridPanel1.ColumnModel.Columns.Add(coldescTipoContrato);


            ////AGREGAR EVENTOS
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_TipoCuenta", "this.getRowsValues({ selectedOnly: true })[0].ID_TipoCuenta", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Evento", "this.getRowsValues({ selectedOnly: true })[0].ID_Evento", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("DesEvento", "this.getRowsValues({ selectedOnly: true })[0].DescEvento", ParameterMode.Raw));

            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_ConfiguracionCorte", "record.data.ID_ConfiguracionCorte", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));

            GridFilters losFiltros = new GridFilters();


            StringFilter filClaveColectiva = new StringFilter();
            filClaveColectiva.DataIndex = "NombreConfiguracion";
            losFiltros.Filters.Add(filClaveColectiva);

            StringFilter filNombreORazonSocial = new StringFilter();
            filNombreORazonSocial.DataIndex = "DescTipoCuenta";
            losFiltros.Filters.Add(filNombreORazonSocial);

            StringFilter filAPaterno = new StringFilter();
            filAPaterno.DataIndex = "DescPeriodo";
            losFiltros.Filters.Add(filAPaterno);

            StringFilter filAMaterno = new StringFilter();
            filAMaterno.DataIndex = "DescEvento";
            losFiltros.Filters.Add(filAMaterno);

            StringFilter filEmail = new StringFilter();
            filEmail.DataIndex = "descTipoContrato";
            losFiltros.Filters.Add(filEmail);

            GridPanel1.Plugins.Add(losFiltros);


        }

        protected void PreparaGridScript()
        {

            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Acciones";
            acciones.Width = 55;
            //acciones.PrepareToolbar.Fn = "prepareToolbar";


            GridCommand elimina = new GridCommand();
            elimina.Icon = Icon.Delete;
            elimina.CommandName = "Eliminar";
            elimina.ToolTip.Text = "Eliminar Movimiento del Script";
            acciones.Commands.Add(elimina);


           Column colID = new Column();
            colID.DataIndex = "ID_Script";
            colID.Header = "ID";
            colID.Sortable = true;
            colID.Hidden = true;
            lasColumnas.Columns.Add(colID);

            Column colEstatus = new Column();
            colEstatus.DataIndex = "EsActiva";
            colEstatus.Header = "Estatus";
            colEstatus.Sortable = true;
           // colEstatus.Hidden = true;
            lasColumnas.Columns.Add(colEstatus);

            Column coldescTipoCuenta = new Column();
            coldescTipoCuenta.DataIndex = "descTipoCuenta";
            coldescTipoCuenta.Header = "Tipo Cuenta";
            coldescTipoCuenta.Sortable = true;
            lasColumnas.Columns.Add(coldescTipoCuenta);

            Column coldescTipoColectiva = new Column();
            coldescTipoColectiva.DataIndex = "descTipoColectiva";
            coldescTipoColectiva.Header = "Tipo Entidad";
            coldescTipoColectiva.Sortable = true;
            lasColumnas.Columns.Add(coldescTipoColectiva);

            Column colEsAbono = new Column();
            colEsAbono.DataIndex = "EsAbono";
            colEsAbono.Header = "EsAbono";
            colEsAbono.Sortable = true;
            lasColumnas.Columns.Add(colEsAbono);

            Column colFormula = new Column();
            colFormula.DataIndex = "Formula";
            colFormula.Header = "Formula de Aplicacion";
            colFormula.Sortable = true;
            lasColumnas.Columns.Add(colFormula);


            Column colValidaSaldo = new Column();
            colValidaSaldo.DataIndex = "ValidaSaldo";
            colValidaSaldo.Header = "Valida Saldo";
            colValidaSaldo.Sortable = true;
            lasColumnas.Columns.Add(colValidaSaldo);


            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Script", "record.data.ID_Script", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));


            GridPanel2.ColumnModel.Columns.Add(acciones);
            GridPanel2.ColumnModel.Columns.Add(colEstatus);
            GridPanel2.ColumnModel.Columns.Add(colID);
            GridPanel2.ColumnModel.Columns.Add(coldescTipoCuenta);
            GridPanel2.ColumnModel.Columns.Add(coldescTipoColectiva);
            GridPanel2.ColumnModel.Columns.Add(colEsAbono);
            GridPanel2.ColumnModel.Columns.Add(colValidaSaldo);
            GridPanel2.ColumnModel.Columns.Add(colFormula);
            

                GridFilters losFiltros = new GridFilters();


            StringFilter filClaveColectiva = new StringFilter();
            filClaveColectiva.DataIndex = "descTipoCuenta";
            losFiltros.Filters.Add(filClaveColectiva);

            StringFilter filNombreORazonSocial = new StringFilter();
            filNombreORazonSocial.DataIndex = "descTipoColectiva";
            losFiltros.Filters.Add(filNombreORazonSocial);

            StringFilter filAPaterno = new StringFilter();
            filAPaterno.DataIndex = "Formula";
            losFiltros.Filters.Add(filAPaterno);

            GridPanel2.Plugins.Add(losFiltros);


        }

        protected ConfigEjecucion NuevaConf()
        {
            try
            {
                ConfigEjecucion unNuevoEjec = new ConfigEjecucion();

                unNuevoEjec.Descripcion = txtDescripcion.Text;
                unNuevoEjec.Clave = txtClave.Text;
                unNuevoEjec.ID_Evento = int.Parse(cmbEvento.Value.ToString());
                unNuevoEjec.ID_Periodo =int.Parse(cmbPeriodo.Value.ToString());
                unNuevoEjec.ID_TipoContrato = int.Parse(cmbTipocontrato.Value.ToString());
                unNuevoEjec.ID_TipoCuenta = int.Parse(cmbTipoCuenta1.Value.ToString());
                unNuevoEjec.Nombre = txtNombre.Text;

                return unNuevoEjec;

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                return new ConfigEjecucion();
            }
        }

        protected ScriptContable NuevoScript()
        {
            try
            {
                ScriptContable unNuevoConfig = new ScriptContable();

                unNuevoConfig.ID_ClaveAplicacion = 0;
                unNuevoConfig.ID_Script = 0;
                unNuevoConfig.ID_TipoColectiva =int.Parse(cmbTipocolectiva.Value.ToString());
                unNuevoConfig.ID_TipoCuenta = int.Parse(cmbTipoCuenta2.Value.ToString());
                unNuevoConfig.ValidaSaldo = bool.Parse(cmbValidaSaldo.Value.ToString());
                unNuevoConfig.EsAbono =bool.Parse(cmbTipoMovimiento.Value.ToString());
                unNuevoConfig.Formula = txtFormula.Text;
                unNuevoConfig.ID_Evento = (int)HttpContext.Current.Session.Contents["ID_Evento"];

                return unNuevoConfig;

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                return new ScriptContable();
            }
        }

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {
                int ID_ConfiguracionCorte = Int32.Parse(e.ExtraParams["ID_ConfiguracionCorte"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                if (ID_ConfiguracionCorte == 0)
                {
                    throw new CAppException(8009, "Por favor, Selecciona el Registro al que deseas aplicar el comando <b> " + EjecutarComando + "</b>");
                }
                Usuario elUser = this.Usuario;

                
                //Solicitar una Confirmacion
                //X.Msg.Confirm("Confirm", "¿Estas seguro de realizar la Accion: <b>" + EjecutarComando + "</b> a la Ficha Numero:<b>" + laFichaSeleccionada.ID_FichaDeposito + "</b> de un Importe de " + String.Format("{0:C}", laFichaSeleccionada.Importe) + "?").Show();


                switch (EjecutarComando)
                {
                    case "Eliminar":
                        DAOCortes.EliminarConfiguracion(ID_ConfiguracionCorte, elUser);
                        break;
                  
                }

                PreparaGridConfig();
                BindDataConfiguracion();
                

            }
            catch (CAppException err)
            {
                X.Msg.Alert("Fichas de Depósito", err.Mensaje()).Show();
            }
            catch (Exception )
            {
                X.Msg.Alert("Fichas de Depósito", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }

        protected void EjecutarComandoScript(object sender, DirectEventArgs e)
        {

            try
            {
                int ID_Script = Int32.Parse(e.ExtraParams["ID_Script"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                if (ID_Script == 0)
                {
                    throw new CAppException(8009, "Por favor, Selecciona el Registro al que deseas aplicar el comando <b> " + EjecutarComando + "</b>");
                }
                Usuario elUser = this.Usuario;


                //Solicitar una Confirmacion
                //X.Msg.Confirm("Confirm", "¿Estas seguro de realizar la Accion: <b>" + EjecutarComando + "</b> a la Ficha Numero:<b>" + laFichaSeleccionada.ID_FichaDeposito + "</b> de un Importe de " + String.Format("{0:C}", laFichaSeleccionada.Importe) + "?").Show();


                switch (EjecutarComando)
                {
                    case "Eliminar":
                        DAOCortes.EliminarScript(ID_Script, elUser);
                        break;

                }

                PreparaGridScript();
                BindDataScript((Int32)HttpContext.Current.Session.Contents["ID_Evento"]);


            }
            catch (CAppException err)
            {
                X.Msg.Alert("Fichas de Depósito", err.Mensaje()).Show();
            }
            catch (Exception )
            {
                X.Msg.Alert("Fichas de Depósito", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }

    }
}