using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Web;
using System.Web.UI.WebControls;

namespace Usuarios
{
    public partial class PermisosAplicaciones : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER PARABILIA PERMISOS APLICACIONES
        private LogHeader LH_ParabPermApps = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabPermApps.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabPermApps.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabPermApps.User = this.Usuario.ClaveUsuario;
            LH_ParabPermApps.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabPermApps);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA PermisosAplicaciones Page_Load()");

                if (!IsPostBack)
                {
                    PreparaGrid();
                    BindDataUsuarios();
                    BindApplicationPerfiles();
                    //BindPerfiles();
                    PreparaGridRoles();
                    PreparaGridTablaValues();
                }

                if (!X.IsAjaxRequest)
                {
                    this.BindDataUsuarios();
                }

                log.Info("TERMINA PermisosAplicaciones Page_Load()");
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

        protected void PreparaGridRoles()
        {
            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column coluserName = new Column();
            coluserName.DataIndex = "RoleId";
            coluserName.Header = "RoleId";
            coluserName.Hidden = true;
            coluserName.Sortable = true;
            lasColumnas.Columns.Add(coluserName);

            Column colNombre = new Column();
            colNombre.DataIndex = "UserId";
            colNombre.Header = "UserId";
            colNombre.Hidden = true;
            colNombre.Sortable = true;
            lasColumnas.Columns.Add(colNombre);

            Column colEmail = new Column();
            colEmail.DataIndex = "RoleName";
            colEmail.Header = "Nombre Rol";
            colEmail.Sortable = true;
            lasColumnas.Columns.Add(colEmail);

            Column colAPaterno = new Column();
            colAPaterno.DataIndex = "Description";
            colAPaterno.Header = "Descripcion";
            colAPaterno.Sortable = true;
            lasColumnas.Columns.Add(colAPaterno);

            GridPanel2.ColumnModel.Columns.Add(coluserName);
            GridPanel2.ColumnModel.Columns.Add(colNombre);
            GridPanel2.ColumnModel.Columns.Add(colEmail);
            GridPanel2.ColumnModel.Columns.Add(colAPaterno);

            GridFilters losFiltros = new GridFilters();

            StringFilter filAPaterno = new StringFilter();
            filAPaterno.DataIndex = "RoleName";
            losFiltros.Filters.Add(filAPaterno);

            StringFilter filAMaterno = new StringFilter();
            filAMaterno.DataIndex = "Description";
            losFiltros.Filters.Add(filAMaterno);

            GridPanel2.Plugins.Add(losFiltros);
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

            Column ColAppId = new Column();
            ColAppId.DataIndex = "ApplicationId";
            ColAppId.Header = "ApplicationId";
            ColAppId.Hidden = true;
            ColAppId.Sortable = true;
            lasColumnas.Columns.Add(ColAppId);

            Column coluserName = new Column();
            coluserName.DataIndex = "UserId";
            coluserName.Header = "UserId";
            coluserName.Hidden = true;
            coluserName.Sortable = true;
            lasColumnas.Columns.Add(coluserName);

            Column colRegisterId = new Column();
            colRegisterId.DataIndex = "RegisterId";
            colRegisterId.Header = "RegisterId";
            colRegisterId.Hidden = true;
            colRegisterId.Sortable = true;
            lasColumnas.Columns.Add(colRegisterId);

            Column ColApp = new Column();
            ColApp.DataIndex = "ApplicationName";
            ColApp.Header = "Aplicacion";
            ColApp.Sortable = true;
            lasColumnas.Columns.Add(ColApp);
            
            Column colNombre = new Column();
            colNombre.DataIndex = "NombreCortoCampo";
            colNombre.Header = "Nombre Corto Campo";
            colNombre.Sortable = true;
            colNombre.Width = Unit.Pixel(100);
            lasColumnas.Columns.Add(colNombre);

            Column colPermitir = new Column();
            colPermitir.DataIndex = "Permitir";
            colPermitir.Header = "Permiso";
            colPermitir.Sortable = true;
            colPermitir.Hidden = true;
            lasColumnas.Columns.Add(colPermitir);

            Column colFieldName = new Column();
            colFieldName.DataIndex = "FieldName";
            colFieldName.Header = "Campo";
            colFieldName.Sortable = true;
            lasColumnas.Columns.Add(colFieldName);

            Column colValue = new Column();
            colValue.DataIndex = "Value";
            colValue.Header = "Valor";
            colValue.Sortable = true;

            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Sentido";
            acciones.Width = 55;
            acciones.PrepareToolbar.Fn = "prepareToolbar";

            GridCommand Reintenta = new GridCommand();
            Reintenta.Icon = Icon.RecordGreen;
            Reintenta.CommandName = "Permitir";
            Reintenta.ToolTip.Text = "Filtrara dejando Pasar los datos que coincidan con el Valor Proporcionado";
            acciones.Commands.Add(Reintenta);

            GridCommand elimina = new GridCommand();
            elimina.Icon = Icon.RecordRed;
            elimina.CommandName = "No Permitir";
            elimina.ToolTip.Text = "No Mostrara los datos que coincidan con el Valor Proporcionado";
            acciones.Commands.Add(elimina);

            lasColumnas.Columns.Add(colValue);
            GridPanel3.ColumnModel.Columns.Add(acciones);
            GridPanel3.ColumnModel.Columns.Add(colID);
            GridPanel3.ColumnModel.Columns.Add(coluserName);
            GridPanel3.ColumnModel.Columns.Add(ColApp);
            GridPanel3.ColumnModel.Columns.Add(colNombre);
            GridPanel3.ColumnModel.Columns.Add(colValue);
            GridPanel3.ColumnModel.Columns.Add(colRegisterId);
            GridPanel3.ColumnModel.Columns.Add(colPermitir);
         
            GridFilters losFiltros = new GridFilters();
            StringFilter filClaveColectiva = new StringFilter();
            filClaveColectiva.DataIndex = "ApplicationName";

            losFiltros.Filters.Add(filClaveColectiva);

            StringFilter filNombreORazonSocial = new StringFilter();
            filNombreORazonSocial.DataIndex = "TableName";
            losFiltros.Filters.Add(filNombreORazonSocial);

            StringFilter filAPaterno = new StringFilter();
            filAPaterno.DataIndex = "Value";
            losFiltros.Filters.Add(filAPaterno);

            StringFilter filAMaterno = new StringFilter();
            filAMaterno.DataIndex = "FieldName";
            losFiltros.Filters.Add(filAMaterno);

            GridPanel3.Plugins.Add(losFiltros);
        }

        private void BindDataUsuarios()
        {
            LogPCI unLog = new LogPCI(LH_ParabPermApps);

            unLog.Info("INICIA ObtieneUsuarios()");
            GridPanel1.GetStore().DataSource = DAOCatalogos.ObtieneUsuarios((this.Usuario).UsuarioTemp,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabPermApps);
            unLog.Info("TERMINA ObtieneUsuarios()");

            GridPanel1.GetStore().DataBind();
        }

        private void BindApplicationPerfiles()
        {
            LogPCI log = new LogPCI(LH_ParabPermApps);

            log.Info("INICIA ListaAplicaciones()");
            SAplicaciones.DataSource = DAOCatalogos.ListaAplicaciones(LH_ParabPermApps);
            log.Info("TERMINA ListaAplicaciones()");

            SAplicaciones.DataBind();
        }

        private void BindPerfiles(Guid UserId)
        {
            LogPCI log = new LogPCI(LH_ParabPermApps);

            log.Info("INICIA ListaPerfiles()");
            SRoles.DataSource = DAOCatalogos.ListaPerfiles(UserId, LH_ParabPermApps);
            log.Info("TERMINA ListaPerfiles()");

            SRoles.DataBind();
        }

        protected void App_Select(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabPermApps);

            try
            {
                BindTabla(Guid.Parse(cmbApp2.Value.ToString()));
                cmbFields.Clear();
                cmbValues.Clear();
                cmbPermiso.Text = "";
            }
            catch (Exception Error)
            {
                log.WarnException(Error);
            }
        }

        protected void Table_Select(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabPermApps);

            try
            {
                BindCampo("*", Guid.Parse(cmbApp2.Value.ToString()));
                cmbValues.Clear();
                cmbPermiso.Text = "";
            }
            catch (Exception Error)
            {
                unLog.WarnException(Error);
            }
        }

        private void BindTabla( Guid appId)
        {
            LogPCI _log = new LogPCI(LH_ParabPermApps);

            _log.Info("INICIA ObtieneTablasFiltro()");
            STables.DataSource = DAOCatalogos.ObtieneTablasFiltro(appId, LH_ParabPermApps);
            _log.Info("TERMINA ObtieneTablasFiltro()");

            STables.DataBind();
        }

        private void BindCampo(String Tabla, Guid AppId)
        {
            LogPCI unLog = new LogPCI(LH_ParabPermApps);

            unLog.Info("INICIA ListaCamposDeTabla()");
            SFields.DataSource = DAOCatalogos.ListaCamposDeTabla(Tabla, AppId, LH_ParabPermApps);
            unLog.Info("TERMINA ListaCamposDeTabla()");

            SFields.DataBind();
        }

        protected void LlenaPosiblesValues(object sender, DirectEventArgs e)
        {
            cmbValues.Clear();
            cmbPermiso.Text = "";
        }


        private void BindPerfilesUsuario(Guid User)
        {
            LogPCI unLog = new LogPCI(LH_ParabPermApps);

            unLog.Info("INICIA ListaPerfilesUsuario()");
            SAppPerfil.DataSource = DAOCatalogos.ListaPerfilesUsuario(User, LH_ParabPermApps);
            unLog.Info("TERMINA ListaPerfilesUsuario()");

            SAppPerfil.DataBind();
        }


        private void BindCamposTablasUsuario( Guid UserId)
        {
            LogPCI _log = new LogPCI(LH_ParabPermApps);

            _log.Info("INICIA ListaTablaValues()");
            SCamposTablasUsuario.DataSource = DAOCatalogos.ListaTablaValues(UserId, LH_ParabPermApps);
            _log.Info("TERMINA ListaTablaValues()");

            SCamposTablasUsuario.DataBind();
        }


        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {
           FormPanel1.Collapsed = true;
        }


        protected void GridUsuarios_DblClik(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabPermApps);
            unLog.Info("PermisosAplicaciones GridUsuarios_DblClik()");

            try
            {
                Guid unvalor = Guid.Parse(e.ExtraParams["UserId"]);
                String Usuario = (String)e.ExtraParams["UserName"];

                HttpContext.Current.Session["UserID"] = unvalor;
                //Poner indicativo del Usuario que se esta Modificando.
                //TabPanel1.SetActiveTab(Panel1);

                //Abrir el Panel de Edicion.
                FormPanel1.Collapsed = false;
                FormPanel1.Title = "Editando el Usuario: [" + Usuario + "]";

                //Llenar el grid de valores de campos de aplicaciones
                BindPerfilesUsuario(unvalor);
                BindCamposTablasUsuario(unvalor);
                BindPerfiles(unvalor);
            }
            catch (Exception err)
            {
                unLog.WarnException(err);
            }
        }


        protected void btnAgregarRole_Click(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabPermApps);

            try
            {
                char[] charsToTrim = { '"', ' ', '\'' };

                Usuario usuarioSeleccionado = new Usuario();

                usuarioSeleccionado.ClaveUsuario = e.ExtraParams["User"].ToString().Trim(charsToTrim);
                usuarioSeleccionado.Nombre = e.ExtraParams["Nombre"].ToString().Trim(charsToTrim);
                usuarioSeleccionado.APaterno = e.ExtraParams["ApPaterno"].ToString().Trim(charsToTrim);
                usuarioSeleccionado.AMaterno = e.ExtraParams["ApMaterno"].ToString().Trim(charsToTrim);
                usuarioSeleccionado.Email = e.ExtraParams["Email"].ToString().Trim(charsToTrim);

                //Se obtiene la clave del rol por asignar en base de datos
                log.Info("INICIA ObtieneClaveRolPorAsignar()");
                string claveRol = DAOUsuario.ObtieneClaveRolPorAsignar(Guid.Parse(cmbPerfil.SelectedItem.Value),
                    this.Usuario, LH_ParabPermApps);
                log.Info("TERMINA ObtieneClaveRolPorAsignar()");

                //###CODIGO OBSOLETO
                //Se realizan prevalidaciones al usuario para poder asignarle ciertos roles
                //string response = ValidacionesPorRol(claveRol, usuarioSeleccionado);
                //if (response != "0")
                //{
                //    X.Msg.Alert("Asignación de Roles", response).Show();
                //    return;
                //}

                //Se asigna el Rol al usuario
                log.Info("INICIA InsertarRol()");
                DAOUsuario.InsertarRol((Guid)HttpContext.Current.Session["UserID"],
                    Guid.Parse(cmbPerfil.Text), LH_ParabPermApps, this.Usuario);
                log.Info("TERMINA InsertarRol()");

                //###CODIGO OBSOLETO
                //Se realizan asignaciones en el Autorizador de los roles necesarios
                //AsignarEnAutorizador(claveRol, usuarioSeleccionado);

                //Actualiza el Grid
                BindPerfilesUsuario((Guid)HttpContext.Current.Session["UserID"]);

                //Actualiza el combo de Perfiles
                cmbPerfil.Clear();
                cmbPerfil.Text = "";
                BindPerfiles((Guid)HttpContext.Current.Session["UserID"]);
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Asignación de Roles", "Ocurrió un error en la asignación del rol.").Show();
            }
        }


        ///###CODIGO OBSOLETO
        /// <summary>
        /// Realiza las validaciones al rol por asignar, por aquellos casos particulares
        /// </summary>
        /// <param name="claveRol">Clave del rol por asignar</param>
        /// <param name="elUsuario">Usuario al que se asignará el rol</param>
        /// <returns>Cadena con el mensaje de error, en caso de que no se cumplan las validaciones,
        /// 0 en caso de éxito</returns>
        //protected string ValidacionesPorRol(string claveRol, Usuario elUsuario)
        //{
        //    LogPCI unLog = new LogPCI(LH_ParabPermApps);
        //    unLog.Info("ValidacionesPorRol()");

        //    try
        //    {
        //        //Si el rol por asignar es el de Recolector o Admin Almacén Diconsa
        //        if ((claveRol == Configuracion.Get(Guid.Parse
        //                (ConfigurationManager.AppSettings["IDApplication"].ToString()), "RolRecolector", LH_ParabPermApps).Valor) ||
        //                (claveRol == Configuracion.Get(Guid.Parse
        //                (ConfigurationManager.AppSettings["IDApplication"].ToString()), "RolAlmacen", LH_ParabPermApps).Valor))
        //        {
        //            //Se validan condiciones
        //            unLog.Info("INICIA ValidaRolDiconsa()");
        //            string resp = LNUsuarios.ValidaRolDiconsa(elUsuario.ClaveUsuario, this.Usuario, LH_ParabPermApps);
        //            unLog.Info("TERMINA ValidaRolDiconsa()");

        //            if (resp != "")
        //            {
        //                return resp;
        //            }
        //        }

        //        return "0";
        //    }

        //    catch (Exception ex)
        //    {
        //        unLog.ErrorException(ex);
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// Valida si el rol asignado necesita asignaciones en el Autorizador, y de ser
        /// así, las solicita
        /// </summary>
        //protected void AsignarEnAutorizador(string cveRolAsignado, Usuario datosUsuario)
        //{
        //    LogPCI unLog = new LogPCI(LH_ParabPermApps);

        //    try
        //    {
        //        //Si el rol insertado es Recolector o Admin Almacén Diconsa
        //        if ((cveRolAsignado == Configuracion.Get(Guid.Parse
        //                (ConfigurationManager.AppSettings["IDApplication"].ToString()), "RolRecolector", LH_ParabPermApps).Valor) ||
        //                (cveRolAsignado == Configuracion.Get(Guid.Parse
        //                (ConfigurationManager.AppSettings["IDApplication"].ToString()), "RolAlmacen", LH_ParabPermApps).Valor))
        //        {
        //            unLog.Info("INICIA AsignaRolDiconsa()");
        //            DAOUsuario.AsignaRolDiconsa(datosUsuario, this.Usuario, LH_ParabPermApps);
        //            unLog.Info("TERMINA AsignaRolDiconsa()");
        //        }

        //        //Si el rol insertado es Operador Loyalty
        //        else if ((cveRolAsignado == Configuracion.Get(Guid.Parse
        //                (ConfigurationManager.AppSettings["IDApplication"].ToString()), "RolOperadorLoyalty", LH_ParabPermApps).Valor))
        //        {
        //            unLog.Info("INICIA CreaColectivaRolOperadorLoyalty()");
        //            DAOUsuario.CreaColectivaRolOperadorLoyalty(datosUsuario, this.Usuario, LH_ParabPermApps);
        //            unLog.Info("TERMINA CreaColectivaRolOperadorLoyalty()");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        unLog.ErrorException(ex);
        //        throw ex;
        //    }
        //}
        ///###CODIGO OBSOLETO

        protected void btnEliminarRole_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabPermApps);

            try
            {
                //Elimina el Rol
                RowSelectionModel sm = GridPanel2.SelectionModel.Primary as RowSelectionModel;

                unLog.Info("INICIA EliminarRol()");
                DAOUsuario.EliminarRol((Guid)HttpContext.Current.Session["UserID"],
                    Guid.Parse(sm.SelectedRow.RecordID), LH_ParabPermApps, this.Usuario);
                unLog.Info("TERMINA EliminarRol()");

                //Actualiza el Grid
                BindPerfilesUsuario((Guid)HttpContext.Current.Session["UserID"]);

                //Actualiza el combo de Perfiles
                cmbPerfil.Clear();
                cmbPerfil.Text = "";
                BindPerfiles((Guid)HttpContext.Current.Session["UserID"]);
            }
            catch (Exception err)
            {
                unLog.WarnException(err);
            }
        }

        protected void btnAgregarTableValue_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabPermApps);

            try
            {
                unLog.Info("INICIA InsertarTablaValue()");
                LNUsuarios.CreaPermisoTableValue((Guid)HttpContext.Current.Session["UserID"], 
                    Guid.Parse(cmbFields.Value.ToString()), cmbValues.Value.ToString(),
                    Boolean.Parse((string)cmbPermiso.Value), LH_ParabPermApps, this.Usuario);
                unLog.Info("TERMINA InsertarTablaValue()");

                FormPanel2.Reset();
                BindCamposTablasUsuario((Guid)HttpContext.Current.Session["UserID"]);
            }
            catch (Exception err)
            {
                unLog.WarnException(err);
            }
        }

        protected void btnEliminarTableValue_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabPermApps);

            try
            {
                //Elimina el Rol
                RowSelectionModel sm = GridPanel3.SelectionModel.Primary as RowSelectionModel;

                log.Info("INICIA EliminarTablaValue()");
                DAOUsuario.EliminarTablaValue((Guid)HttpContext.Current.Session["UserID"],
                    Int64.Parse(sm.SelectedRow.RecordID), LH_ParabPermApps, this.Usuario);
                log.Info("TERMINA EliminarTablaValue()");

                //Actualiza el Grid
                BindCamposTablasUsuario((Guid)HttpContext.Current.Session["UserID"]);
            }
            catch (Exception err)
            {
                log.WarnException(err);
            }
        }

        protected void RefreshGridUsuarios(object sender, StoreRefreshDataEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabPermApps);
            unLog.Info("PermisosAplicaciones RefreshGridUsuarios()");

            try
            {
                PreparaGrid();
                BindDataUsuarios();
            }
            catch (Exception err)
            {
                unLog.WarnException(err);
            }
        }

        protected void RefreshGridRoles(object sender, StoreRefreshDataEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabPermApps);
            log.Info("PermisosAplicaciones RefreshGridRoles()");

            try
            {
                PreparaGridRoles();
                BindPerfilesUsuario((Guid)HttpContext.Current.Session.Contents["UserID"]);
            }
            catch (Exception err)
            {
                log.WarnException(err);
            }
        }

        protected void RefreshGridValores(object sender, StoreRefreshDataEventArgs e)
        {
            LogPCI elLog = new LogPCI(LH_ParabPermApps);
            elLog.Info("PermisosAplicaciones RefreshGridValores()");

            try
            {
                PreparaGridTablaValues();
                BindCamposTablasUsuario((Guid)HttpContext.Current.Session.Contents["UserID"]);
            }
            catch (Exception err)
            {
                elLog.WarnException(err);
            }
        }
    }
}