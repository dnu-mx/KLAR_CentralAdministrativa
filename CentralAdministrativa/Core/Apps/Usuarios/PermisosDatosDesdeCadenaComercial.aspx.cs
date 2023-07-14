using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

namespace Usuarios
{
    public partial class PermisosDatosDesdeCadenaComercial : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER PERMISOS DATOS
        private LogHeader LH_PermDatos = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_PermDatos.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_PermDatos.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_PermDatos.User = this.Usuario.ClaveUsuario;
            LH_PermDatos.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_PermDatos);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA PermisosDatosDesdeCadenaComercial Page_Load()");

                if (!IsPostBack)
                {
                    PreparaGrid();
                    BindDataUsuarios();
                    PreparaGridTablaValues();
                }

                if (!X.IsAjaxRequest)
                {
                    this.BindDataUsuarios();
                }

                log.Info("TERMINA PermisosDatosDesdeCadenaComercial Page_Load()");
            }

            catch (Exception err)
            {
                log.ErrorException(err);
                errRedirect = "../ErrorInicializarPagina.aspx";
            }

            finally
            {
                if (!string.IsNullOrEmpty(errRedirect))
                {
                    Response.Redirect(errRedirect, false);

                    Response.Flush();
                    Response.Close();
                }
            }
        }

        private void AgregaRecordFiles()
        {
            SAllUser.AddField(new RecordField("UserId"));
            SAllUser.AddField(new RecordField("UserName"));
            SAllUser.AddField(new RecordField("Nombre"));
            SAllUser.AddField(new RecordField("Apaterno"));
            SAllUser.AddField(new RecordField("Amaterno"));
            SAllUser.AddField(new RecordField("Email"));
            SAllUser.AddField(new RecordField("Estatus"));

        }

        protected void PreparaGrid()
        {
            AgregaRecordFiles();

            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colID = new Column();
            colID.DataIndex = "UserId";
            colID.Header = "ID";
            colID.Sortable = true;
            colID.Hidden = true;
            lasColumnas.Columns.Add(colID);

            Column coluserName = new Column();
            coluserName.DataIndex = "UserName";
            coluserName.Header = "Usuario";
            coluserName.Sortable = true;
            lasColumnas.Columns.Add(coluserName);

            Column colNombre = new Column();
            colNombre.DataIndex = "Nombre";
            colNombre.Header = "Nombre";
            colNombre.Sortable = true;
            lasColumnas.Columns.Add(colNombre);

            Column colAPaterno = new Column();
            colAPaterno.DataIndex = "Apaterno";
            colAPaterno.Header = "ApellidoPaterno";
            colAPaterno.Sortable = true;
            lasColumnas.Columns.Add(colAPaterno);

            Column colAMaterno = new Column();
            colAMaterno.DataIndex = "Amaterno";
            colAMaterno.Header = "Apellido Materno";
            colAMaterno.Sortable = true;
            lasColumnas.Columns.Add(colAMaterno);

            Column colEmail = new Column();
            colEmail.DataIndex = "Email";
            colEmail.Header = "Email";
            colEmail.Sortable = true;
            lasColumnas.Columns.Add(colEmail);

            GridPanel1.ColumnModel.Columns.Add(colID);
            GridPanel1.ColumnModel.Columns.Add(coluserName);
            GridPanel1.ColumnModel.Columns.Add(colNombre);
            GridPanel1.ColumnModel.Columns.Add(colAPaterno);
            GridPanel1.ColumnModel.Columns.Add(colAMaterno);
            GridPanel1.ColumnModel.Columns.Add(colEmail);

            ////AGREGAR EVENTOS
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("UserId", "this.getRowsValues({ selectedOnly: true })[0].UserId", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("UserName", "this.getRowsValues({ selectedOnly: true })[0].UserName", ParameterMode.Raw));

            GridFilters losFiltros = new GridFilters();

            StringFilter filClaveColectiva = new StringFilter();
            filClaveColectiva.DataIndex = "UserName";
            losFiltros.Filters.Add(filClaveColectiva);

            StringFilter filNombreORazonSocial = new StringFilter();
            filNombreORazonSocial.DataIndex = "Nombre";
            losFiltros.Filters.Add(filNombreORazonSocial);

            StringFilter filAPaterno = new StringFilter();
            filAPaterno.DataIndex = "Apaterno";
            losFiltros.Filters.Add(filAPaterno);

            StringFilter filAMaterno = new StringFilter();
            filAMaterno.DataIndex = "Amaterno";
            losFiltros.Filters.Add(filAMaterno);

            StringFilter filEmail = new StringFilter();
            filEmail.DataIndex = "Email";
            losFiltros.Filters.Add(filEmail);

            GridPanel1.Plugins.Add(losFiltros);
        }

      
        protected void PreparaGridTablaValues()
        {
            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colID = new Column();
            colID.DataIndex = "TableId";
            colID.Header = "TableId";
            colID.Sortable = true;
            colID.Hidden = true;
            lasColumnas.Columns.Add(colID);

            Column colRegisterId = new Column();
            colRegisterId.DataIndex = "RegisterId";
            colRegisterId.Header = "RegisterId";
            colRegisterId.Hidden = true;
            colRegisterId.Sortable = true;
            lasColumnas.Columns.Add(colRegisterId);

            Column coluserName = new Column();
            coluserName.DataIndex = "Value";
            coluserName.Header = "Valor";
            coluserName.Sortable = true;
            lasColumnas.Columns.Add(coluserName);

            Column ColPermitir = new Column();
            ColPermitir.DataIndex = "Permitir";
            ColPermitir.Header = "Permitir";
            ColPermitir.Sortable = true;
            lasColumnas.Columns.Add(ColPermitir);

            Column ColAppId = new Column();
            ColAppId.DataIndex = "ValueDescription";
            ColAppId.Header = "Descripcion";
            ColAppId.Sortable = true;
            lasColumnas.Columns.Add(ColAppId);

            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Sentido";
            acciones.Width = 55;
            acciones.PrepareToolbar.Fn = "prepareToolbar";

            GridCommand Reintenta = new GridCommand();
            Reintenta.Icon = Icon.RecordGreen;
            Reintenta.CommandName = "NoPermitir";
            Reintenta.ToolTip.Text = "No permitir al Usuario seleccionado vista de los datos relacionados al valor";
            acciones.Commands.Add(Reintenta);

            GridCommand elimina = new GridCommand();
            elimina.Icon = Icon.RecordRed;
            elimina.CommandName = "Permitir";
            elimina.ToolTip.Text = "Permitir al Usuario seleccionado vista de los datos relacioandos con el valor";
            acciones.Commands.Add(elimina);

            GridPanel3.ColumnModel.Columns.Add(acciones);
            GridPanel3.ColumnModel.Columns.Add(ColPermitir);
            GridPanel3.ColumnModel.Columns.Add(colRegisterId);
            GridPanel3.ColumnModel.Columns.Add(coluserName);
            GridPanel3.ColumnModel.Columns.Add(ColAppId);
            GridPanel3.ColumnModel.Columns.Add(colID);

            GridPanel3.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("RegisterId", "record.data.RegisterId", ParameterMode.Raw));
            GridPanel3.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Value", "record.data.Value", ParameterMode.Raw));
            GridPanel3.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));

            ////AGREGAR EVENTOS
            GridFilters losFiltros = new GridFilters();
            StringFilter filClaveColectiva = new StringFilter();
            filClaveColectiva.DataIndex = "ApplicationName";

            losFiltros.Filters.Add(filClaveColectiva);

            BooleanFilter filPermitir = new BooleanFilter();
            filPermitir.DataIndex = "Permitir";
            losFiltros.Filters.Add(filPermitir);

            StringFilter filNombreORazonSocial = new StringFilter();
            filNombreORazonSocial.DataIndex = "TableName";
            losFiltros.Filters.Add(filNombreORazonSocial);

            StringFilter filAPaterno = new StringFilter();
            filAPaterno.DataIndex = "Value";
            losFiltros.Filters.Add(filAPaterno);

            StringFilter filAMaterno = new StringFilter();
            filAMaterno.DataIndex = "ValueDescription";
            losFiltros.Filters.Add(filAMaterno);

            GridPanel3.Plugins.Add(losFiltros);
        }

        private void BindDataUsuarios()
        {
            LogPCI pCI = new LogPCI(LH_PermDatos);

            try
            {
                pCI.Info("INICIA ObtieneUsuarios()");
                GridPanel1.GetStore().DataSource = StoreUsuarios.DataSource =
                    DAOCatalogos.ObtieneUsuarios(this.Usuario.UsuarioTemp,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_PermDatos);
                pCI.Info("TERMINA ObtieneUsuarios()");

                GridPanel1.GetStore().DataBind();
                StoreUsuarios.DataBind();
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw ex;
            }
        }

        private void BindApplication(Guid ID_Usuario)
        {
            LogPCI logPCI = new LogPCI(LH_PermDatos);

            try
            {
                logPCI.Info("INICIA ListaAplicacionesUsuario()");
                SAplicaciones.DataSource = DAOCatalogos.ListaAplicacionesUsuario(ID_Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_PermDatos);
                logPCI.Info("TERMINA ListaAplicacionesUsuario()");

                SAplicaciones.DataBind();
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw ex;
            }
        }

        protected void App_Select(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_PermDatos);
            log.Info("PermisosDatosDesdeCadenaComercial App_Select()");

            try
            {
                BindTabla(Guid.Parse(cmbApp2.Value.ToString()));
               
                SCamposTablasUsuario.RemoveAll();
            }
            catch (Exception Error)
            {
                log.WarnException(Error);
            }
        }

        protected void Table_Select(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_PermDatos);
            unLog.Info("PermisosDatosDesdeCadenaComercial Table_Select()");

            try
            {
                BindCampo("*", Guid.Parse(cmbApp2.Value.ToString()));
            }
            catch (Exception Error)
            {
                unLog.WarnException(Error);
            }
        }

        private void BindTabla(Guid appId)
        {
            LogPCI pCI = new LogPCI(LH_PermDatos);

            try
            {
                pCI.Info("INICIA ObtieneTablasFiltro()");
                STables.DataSource = DAOCatalogos.ObtieneTablasFiltro(appId, LH_PermDatos);
                pCI.Info("TERMINA ObtieneTablasFiltro()");

                STables.DataBind();
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw ex;
            }
        }

        private void BindCampo(String Tabla, Guid AppId)
        {
            LogPCI log = new LogPCI(LH_PermDatos);

            try
            {
                log.Info("INICIA ListaCamposDeTabla()");
                SFields.DataSource = DAOCatalogos.ListaCamposDeTabla(Tabla, AppId, LH_PermDatos);
                log.Info("TERMINA ListaCamposDeTabla()");

                SFields.DataBind();
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }

        protected void LlenaPosiblesValues(object sender, DirectEventArgs e)
        {
            BindValores(Guid.Parse(cmbFields.Value.ToString()), 
                (Guid)HttpContext.Current.Session["UserID"], txtFiltro.Text);
        }

        private void BindValores(Guid IdValue, Guid UserId, String FiltroValue)
        {
            LogPCI unLog = new LogPCI(LH_PermDatos);

            try
            {
                unLog.Info("INICIA ObtieneFiltro()");
                Filtro unFiltro = DAOFiltro.ObtieneFiltro(IdValue, LH_PermDatos);
                unLog.Info("TERMINA ObtieneFiltro()");

                unLog.Info("INICIA ListaCombinacionTablaValues()");
                List<ValorFiltro> lista = DAOCatalogos.ListaCombinacionTablaValues(UserId, IdValue, 
                    DAOFiltro.SeleccionaPosiblesConfiguradosPublico(Configuracion.Get(unFiltro.AppID, unFiltro.ConexionName).Valor, 
                    unFiltro.StoredProcedure, this.Usuario, unFiltro.AppID, FiltroValue), FiltroValue, LH_PermDatos);
                unLog.Info("TERMINA ListaCombinacionTablaValues()");

                if (lista.Count < 1)
                {
                    X.Msg.Alert("Búsqueda de Filtros", "No existen coincidencias con la búsqueda solicitada.").Show();
                    return;
                }
                else
                {
                    SCamposTablasUsuario.DataSource = lista;
                    SCamposTablasUsuario.DataBind();
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Búsqueda de Filtros", "Ocurrio un Error en la Búsqueda: " + ex.Message).Show();
            }
        }

        protected void btnClonar_Click(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_PermDatos);

            try
            {
                if (cmbUsuarioOrigen.SelectedIndex == -1)
                {
                    X.Msg.Alert("Clonación de Vistas", "Selecciona el Usuario Origen").Show();
                    return;
                }

                unLog.Info("INICIA ClonarPermisosUsuario()");
                LNUsuarios.ClonarUsuario(Guid.Parse(cmbUsuarioOrigen.SelectedItem.Value),
                    Guid.Parse(HttpContext.Current.Session["UserID"].ToString()), LH_PermDatos, this.Usuario);
                unLog.Info("TERMINA ClonarPermisosUsuario()");

                X.Msg.Notify("Clonación de Vistas", "Se copiaron los permisos de vista con éxito").Show();
                X.Msg.Notify("Clonación de Vistas", "<br />  <br /> <b>  A U T O R I Z A D O  </b> <br />  <br /> ").Show();
            }
            catch (Exception err)
            {
                unLog.ErrorException(err);
                X.Msg.Notify("Clonación de Vistas", "No se pudieron clonar las vistas").Show();
                X.Msg.Notify("Clonación de Vistas", "<br />  <br /> <b> E R R O R </b> <br />  <br /> ").Show();
            }
        }


        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {
            FormPanelAsignarVistas.Collapsed = true;
            SCamposTablasUsuario.RemoveAll();
        }

        protected void GridUsuarios_DblClik(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_PermDatos);
            unLog.Info("PermisosDatosDesdeCadenaComercial GridUsuarios_DblClik()");

            try
            {
                Guid unvalor = Guid.Parse(e.ExtraParams["UserId"]);
                String Usuario = (String)e.ExtraParams["UserName"];

                HttpContext.Current.Session["UserID"] = unvalor;
                
                //Abrir el Panel de Edicion.
                FormPanelAsignarVistas.Reset();
                FormPanelAsignarVistas.Collapsed = false;

                FormPanelAsignarVistas.Title = "Asignar Vistas al Usuario: [" + Usuario + "]";

                SCamposTablasUsuario.RemoveAll();
                
                BindApplication(unvalor);

                PreparaGridTablaValues();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
            }
        }
                
        protected void RefreshGridUsuarios(object sender, StoreRefreshDataEventArgs e)
        {
            LogPCI log = new LogPCI(LH_PermDatos);
            log.Info("PermisosDatosDesdeCadenaComercial RefreshGridUsuarios");

            try
            {
                PreparaGrid();
                BindDataUsuarios();
            }
            catch (Exception err)
            {
                log.WarnException(err);
            }
        }
        
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_PermDatos);

            try
            {
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                Guid IDField = Guid.Parse(cmbFields.Value.ToString());
                String Value = (String)e.ExtraParams["Value"];


                switch (EjecutarComando)
                {
                    case "Permitir":
                        unLog.Info("INICIA InsertarTablaValue()");
                        LNUsuarios.CreaPermisoTableValue((Guid)HttpContext.Current.Session["UserID"], 
                            Guid.Parse(cmbFields.Value.ToString()), Value, true, LH_PermDatos, this.Usuario);
                        unLog.Info("TERMINA InsertarTablaValue()");
                        break;

                    case "NoPermitir":
                        //Elimina el permiso
                        Int64 registerId = Int64.Parse(e.ExtraParams["RegisterId"]);
                        unLog.Info("INICIA EliminarTablaValue()");
                        DAOUsuario.EliminarTablaValue((Guid)HttpContext.Current.Session["UserID"],
                            registerId, LH_PermDatos, this.Usuario);
                        unLog.Info("TERMINA EliminarTablaValue()");

                        break;
                }

                BindValores(IDField, (Guid)HttpContext.Current.Session["UserID"], txtFiltro.Text);
            }
            catch (Exception err)
            {
                unLog.ErrorException(err);
                X.Msg.Alert("Acción", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }

        protected void RefreshGridValores(object sender, StoreRefreshDataEventArgs e)
        { 
            BindValores(Guid.Parse(cmbFields.Value.ToString()),
                (Guid)HttpContext.Current.Session["UserID"], txtFiltro.Text);
        }     
    }
}