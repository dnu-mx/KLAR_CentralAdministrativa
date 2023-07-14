using DALAutorizador.BaseDatos;
using DALAutorizador.LogicaNegocio;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Web;

namespace Cortador
{
    public partial class GenerarCorte : DALCentralAplicaciones.PaginaBaseCAPP
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
              

                if (!IsPostBack)
                {
                    PreparaGridConfig();
                    BindDataConfiguracion();

                    PreparaGridCuentas();
                }

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }



        private void BindDataCuentas(Int32 ID_ConfiguracionCorte)
        {
            GridPanel2.GetStore().DataSource = DAOCortes.ObtieneCuentasParaProcesoAutomatico(ID_ConfiguracionCorte,this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel2.GetStore().DataBind();
        }

        protected void RefreshGridCuentas(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                PreparaGridCuentas();
                BindDataCuentas((Int32)HttpContext.Current.Session.Contents["ID_ConfiguracionCorte"]);
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Generar Cortes", err.Mensaje()).Show();
            }
            catch (Exception )
            {
                X.Msg.Alert("Generar Cortes", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }

        protected void RefreshGridCortes(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                PreparaGridConfig();
                BindDataConfiguracion();
               
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Generar Cortes", err.Mensaje()).Show();
            }
            catch (Exception )
            {
                X.Msg.Alert("Generar Cortes", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }

        private void BindDataConfiguracion()
        {
            GridPanel1.GetStore().DataSource = DAOCortes.ObtieneConfiguracion(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel1.GetStore().DataBind();
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
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_ConfiguracionCorte", "this.getRowsValues({ selectedOnly: true })[0].ID_ConfiguracionCorte", ParameterMode.Raw));
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


        protected void btnGuardarConfig_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {

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
            try
            {
                Panel5.Collapsed = true;
            }
            catch (Exception )
            {
            }
        }

        protected void GridConfig_DblClik(object sender, DirectEventArgs e)
        {
            try
            {

                Int32 ID_ConfiguracionCorte = Int32.Parse(e.ExtraParams["ID_ConfiguracionCorte"] == null ? "0" : e.ExtraParams["ID_ConfiguracionCorte"]);
                Int32 ID_TipoContrato = Int32.Parse(e.ExtraParams["ID_TipoContrato"] == null ? "0" : e.ExtraParams["ID_TipoContrato"]);
                Int32 IDEvento = Int32.Parse(e.ExtraParams["ID_Evento"] == null ? "0" : e.ExtraParams["ID_Evento"]);
                String DesEvento = e.ExtraParams["DesEvento"] == null ? "" : e.ExtraParams["DesEvento"];
                HttpContext.Current.Session.Add("ID_Evento", IDEvento);
                HttpContext.Current.Session.Add("ID_ConfiguracionCorte", ID_ConfiguracionCorte);

//                Panel5.Title = "Scripts de Ejecucion Periódica [" + DesEvento + "]";
                Panel5.Collapsed = false;


                PreparaGridCuentas();
                BindDataCuentas(ID_ConfiguracionCorte);


            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                X.Msg.Alert("Mostrar Cuentas para corte", "Error al Mostrar el Script Contable").Show();
                return;
            }

        }

        protected void btnCancelarConfig_Click(object sender, EventArgs e)
        {
            
        }

        protected void btnGuardarScript_Click(object sender, EventArgs e)
        {
            try
            {

              

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
        
        }

        protected void PreparaGridCuentas()
        {

            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Acciones";
            acciones.Width = 55;
            //acciones.PrepareToolbar.Fn = "prepareToolbar";


            GridCommand elimina = new GridCommand();
            elimina.Icon = Icon.ControlPlayBlue;
            elimina.CommandName = "Iniciar";
            elimina.ToolTip.Text = "Iniciar el Corte a la Cuenta Actual";
            acciones.Commands.Add(elimina);


            Column colID = new Column();
            colID.DataIndex = "ID_Cuenta";
            colID.Header = "ID";
            colID.Sortable = true;
            //colID.Hidden = true;
            lasColumnas.Columns.Add(colID);

            Column colClaveTipocontrato = new Column();
            colClaveTipocontrato.DataIndex = "ClaveTipocontrato";
            colClaveTipocontrato.Header = "ClaveTipocontrato";
            colClaveTipocontrato.Width = 100;
            colClaveTipocontrato.Sortable = true;
            colClaveTipocontrato.Hidden = true;
            lasColumnas.Columns.Add(colClaveTipocontrato);

            Column coldescTipoContrato = new Column();
            coldescTipoContrato.DataIndex = "descTipoContrato";
            coldescTipoContrato.Header = "descTipoContrato";
            coldescTipoContrato.Width = 150;
            coldescTipoContrato.Sortable = true;
            coldescTipoContrato.Hidden = true;
            lasColumnas.Columns.Add(coldescTipoContrato);

            Column colClaveEvento = new Column();
            colClaveEvento.DataIndex = "ClaveEvento";
            colClaveEvento.Header = "ClaveEvento";
            colClaveEvento.Width = 200;
            colClaveEvento.Sortable = true;
            colClaveEvento.Hidden = true;
            lasColumnas.Columns.Add(colClaveEvento);

            Column colDescEvento = new Column();
            colDescEvento.DataIndex = "DescEvento";
            colDescEvento.Header = "DescEvento";
            colDescEvento.Sortable = true;
            colDescEvento.Hidden = true;

            lasColumnas.Columns.Add(colDescEvento);

            Column colCadenaComercial = new Column();
            colCadenaComercial.DataIndex = "CadenaComercial";
            colCadenaComercial.Header = "Cadena Comercial";
            colCadenaComercial.Sortable = true;
            //colCadenaComercial.Hidden = true;
            lasColumnas.Columns.Add(colCadenaComercial);

            Column colDescTipoCuenta = new Column();
            colDescTipoCuenta.DataIndex = "DescTipoCuenta";
            colDescTipoCuenta.Header = "Tipo Cuenta";
            colDescTipoCuenta.Width = 200;
            colDescTipoCuenta.Sortable = true;
            //colDescTipoCuenta.Hidden = true;
            lasColumnas.Columns.Add(colDescTipoCuenta);

            Column colDescPeriodo = new Column();
            colDescPeriodo.DataIndex = "DescPeriodo";
            colDescPeriodo.Header = "Periodo";
            colDescPeriodo.Width = 200;
            colDescPeriodo.Sortable = true;
            colDescPeriodo.Hidden = true;
            lasColumnas.Columns.Add(colDescPeriodo);

            Column colCuentaHabiente = new Column();
            colCuentaHabiente.DataIndex = "CuentaHabiente";
            colCuentaHabiente.Header = "CuentaHabiente";
            colCuentaHabiente.Width = 200;
            colCuentaHabiente.Sortable = true;
            lasColumnas.Columns.Add(colCuentaHabiente);

            Column colClaveEjecucion = new Column();
            colClaveEjecucion.DataIndex = "ClaveEjecucion";
            colClaveEjecucion.Header = "ClaveEjecucion";
            colClaveEjecucion.Width = 200;
            colClaveEjecucion.Sortable = true;
            colClaveEjecucion.Hidden = true;
            lasColumnas.Columns.Add(colClaveEjecucion);

            Column colDescEjecucion = new Column();
            colDescEjecucion.DataIndex = "DescEjecucion";
            colDescEjecucion.Header = "DescEjecucion";
            colDescEjecucion.Width = 200;
            colDescEjecucion.Sortable = true;
            colDescEjecucion.Hidden = true;

            lasColumnas.Columns.Add(colDescEjecucion);

            Column colID_Evento = new Column();
            colID_Evento.DataIndex = "ID_Evento";
            colID_Evento.Header = "Contrato";
            colID_Evento.Width = 200;
            colID_Evento.Sortable = true;
            colID_Evento.Hidden = true;
            lasColumnas.Columns.Add(colID_Evento);

            Column colID_Cuenta = new Column();
            colID_Cuenta.DataIndex = "ID_Cuenta";
            colID_Cuenta.Header = "ID_Cuenta";
            colID_Cuenta.Width = 200;
            colID_Cuenta.Sortable = true;
            colID_Cuenta.Hidden = true;
            lasColumnas.Columns.Add(colID_Cuenta);

            Column colID_EstatusConfiguracion = new Column();
            colID_EstatusConfiguracion.DataIndex = "ID_EstatusConfiguracion";
            colID_EstatusConfiguracion.Header = "ID_EstatusConfiguracion";
            colID_EstatusConfiguracion.Width = 200;
            colID_EstatusConfiguracion.Sortable = true;
            colID_EstatusConfiguracion.Hidden = true;
            lasColumnas.Columns.Add(colID_EstatusConfiguracion);


            GridPanel2.ColumnModel.Columns.Add(acciones);
            GridPanel2.ColumnModel.Columns.Add(colID);
            GridPanel2.ColumnModel.Columns.Add(colDescTipoCuenta);
            GridPanel2.ColumnModel.Columns.Add(colCadenaComercial);
            GridPanel2.ColumnModel.Columns.Add(colClaveEjecucion);
            GridPanel2.ColumnModel.Columns.Add(colDescEjecucion);
            //GridPanel1.ColumnModel.Columns.Add(colClaveEvento);
            GridPanel2.ColumnModel.Columns.Add(colDescEvento);
            //GridPanel1.ColumnModel.Columns.Add(colClaveTipocontrato);
            GridPanel2.ColumnModel.Columns.Add(coldescTipoContrato);

            GridPanel2.ColumnModel.Columns.Add(colDescEjecucion);

            GridPanel2.ColumnModel.Columns.Add(colCuentaHabiente);
            
            GridPanel2.ColumnModel.Columns.Add(colDescPeriodo);
            GridPanel2.ColumnModel.Columns.Add(coldescTipoContrato);


         
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_ConfiguracionCorte", "record.data.ID_ConfiguracionCorte", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Cuenta", "record.data.ID_Cuenta", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_CadenaComercial", "record.data.ID_CadenaComercial", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_CuentaHabiente", "record.data.ID_CuentaHabiente", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_TipoContrato", "record.data.ID_TipoContrato", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("DescEjecucion", "record.data.DescEjecucion", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ClaveEvento", "record.data.ClaveEvento", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("DiasComprendidos", "record.data.DiasComprendidos", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("DiasMes", "record.data.DiasMes", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("MesesAnio", "record.data.MesesAnio", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Corte", "record.data.ID_Corte", ParameterMode.Raw));


            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));

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

        protected void EjecutarComandoCuenta(object sender, DirectEventArgs e)
        {

            try
            {
                int ID_ConfiguracionCorte = Int32.Parse(e.ExtraParams["ID_ConfiguracionCorte"]);
                int ID_Cuenta = Int32.Parse(e.ExtraParams["ID_Cuenta"]);
                int ID_CadenaComercial = Int32.Parse(e.ExtraParams["ID_CadenaComercial"]);
                int ID_CuentaHabiente = Int32.Parse(e.ExtraParams["ID_CuentaHabiente"]);
                int ID_TipoContrato = Int32.Parse(e.ExtraParams["ID_TipoContrato"]);
                String DescEjecucion = (string)e.ExtraParams["DescEjecucion"];
                String ClaveEvento = (string)e.ExtraParams["ClaveEvento"];
                Int64 ID_Corte = Int64.Parse(e.ExtraParams["ID_Corte"]);

                int DiasComprendidos = Int32.Parse(e.ExtraParams["DiasComprendidos"]);
                String DiasMes = (string)e.ExtraParams["DiasMes"];
                String MesesAnio = (string)e.ExtraParams["MesesAnio"];

               
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                if (ID_ConfiguracionCorte == 0)
                {
                    throw new CAppException(8009, "Por favor, Selecciona el Registro al que deseas aplicar el comando <b> " + EjecutarComando + "</b>");
                }
                Usuario elUser = this.Usuario;


                switch (EjecutarComando)
                {
                    case "Iniciar":
                        //DAOCortes.EliminarScript(ID_ConfiguracionCorte, elUser);
                        int respuesta = LNCortes.IniciarCorte(DiasComprendidos, DiasMes, MesesAnio, ID_ConfiguracionCorte, ID_Cuenta, ID_CadenaComercial, ID_CuentaHabiente, ID_TipoContrato, DescEjecucion, ClaveEvento, ID_Corte);

                        if (respuesta != 0)
                        {
                            throw new CAppException(respuesta, "No se Ejecuto el Proceso en la Cuenta Seleccionada. </br> Codigo Respuesta:<b> [" + respuesta + "]</b>");
                        }
                        else
                        {
                            X.Msg.Notify("Ejecución Manual", "Resultados de la Operación <br /> <b>" + DescEjecucion + "</b> " + "<br/> No. Cuenta: <b> " + ID_Cuenta.ToString().PadLeft(8,'0') + "</b>").Show();
                            X.Msg.Notify("Ejecución Manual", "Ejecución de Proceso Manual <br />  <br /> <b>  E X I T O S O  </b> <br />  <br /> ").Show();
                        }
                        break;

                }

                PreparaGridCuentas();
                BindDataCuentas(ID_ConfiguracionCorte);

                

       

            }
            catch (CAppException err)
            {
                X.Msg.Alert("Ejecución de Evento", err.Mensaje()).Show();
            }
            catch (Exception )
            {
                X.Msg.Alert("Ejecución de Evento", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }

 
       
    }
}