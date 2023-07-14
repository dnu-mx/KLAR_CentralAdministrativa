using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Web;
using System.Web.Security;

namespace Usuarios
{
    public partial class EditarUsuarioDesdeCadenaComercial : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER EDIT USER
        private LogHeader LH_EditUsr = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_EditUsr.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_EditUsr.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_EditUsr.User = this.Usuario.ClaveUsuario;
            LH_EditUsr.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_EditUsr);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA EditarUsuarioDesdeCadenaComercial Page_Load()");

                if (!IsPostBack)
                {
                    LlenaTiposColectiva();
                }

                if (!IsPostBack)
                {
                    PreparaGrid();
                    BindDataUsuarios();
                }

                log.Info("TERMINA EditarUsuarioDesdeCadenaComercial Page_Load()");
            }

            catch (CAppException)
            {
                errRedirect = "../ErrorInicializarPagina.aspx";
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

        public void LlenaTiposColectiva()
        {
            LogPCI unLog = new LogPCI(LH_EditUsr);

            try
            {
                unLog.Info("INICIA ListaTipoColectivaFiltroNivelCadena()");
                SCTipoColectiva.DataSource = DALAutorizador.BaseDatos.DAOCatalogos.ListaTipoColectivaFiltroNivelCadena(
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_EditUsr);
                unLog.Info("TERMINA ListaTipoColectivaFiltroNivelCadena()");

                SCTipoColectiva.DataBind();
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        protected void TipoColectiva_Select(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_EditUsr);

            try
            {
                pCI.Info("INICIA ListaColectivaCadenasComercial()");
                SCColectiva.DataSource = DALAutorizador.BaseDatos.DAOCatalogos.ListaColectivaCadenasComercial(
                    (String)cmbTipoColectiva.Value, this.Usuario, 
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_EditUsr);
                pCI.Info("TERMINA ListaColectivaCadenasComercial()");

                SCColectiva.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Cadena Comercial", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Cadena Comercial", "Ocurrió un error al consultar las Cadenas Comerciales.").Show();
            }
        }

        private void BindDataUsuarios()
        {
            LogPCI log = new LogPCI(LH_EditUsr);

            try
            {
                log.Info("INICIA ObtieneUsuarios()");
                GridPanel1.GetStore().DataSource = DAOCatalogos.ObtieneUsuarios((this.Usuario).UsuarioTemp,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_EditUsr);
                log.Info("TERMINA ObtieneUsuarios()");

                GridPanel1.GetStore().DataBind();
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

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_EditUsr);

            try
            {               
                try
                {
                    if (txtPass1.Text.Equals(txtPass2.Text))
                    {
                        if (txtAmaterno.Text == "" ||
                           txtApaterno.Text == "" ||
                           txtNombre.Text == "" ||
                           txtEmail.Text == "" ||
                           txtPass1.Text == "" ||
                           txtPass2.Text == "" ||
                           txtUsuario.Text == "")
                        {
                            X.Msg.Alert("Actualizando Usuario", "Campos vacíos. Por favor, proporciona todos los campos").Show();
                            return;
                        }

                        try
                        {
                            unLog.Info("INICIA ObtieneCaracteristicasUsuario()");
                            Usuario nuevoUsuario = DAOUsuario.ObtieneCaracteristicasUsuario(txtUsuario.Text, LH_EditUsr);
                            unLog.Info("TERMINA ObtieneCaracteristicasUsuario()");

                            nuevoUsuario.AMaterno = txtAmaterno.Text;
                            nuevoUsuario.APaterno = txtApaterno.Text;
                            nuevoUsuario.Nombre = txtNombre.Text;
                            nuevoUsuario.Email = txtEmail.Text;
                            nuevoUsuario.Password = txtPass1.Text;
                            nuevoUsuario.ID_Colectiva = Int64.Parse((String)cmbCadenaComercial2.Value);

                            unLog.Info("INICIA ActualizaUsuario()");
                            DAOUsuario.ActualizaUsuario(nuevoUsuario, LH_EditUsr, this.Usuario, "_EditarComercial");
                            unLog.Info("TERMINA ActualizaUsuario()");

                            //Si el cambio de datos fue para otro usuario diferente al que está en sesión
                            if (nuevoUsuario.UsuarioId != this.Usuario.UsuarioId)
                            {
                                //Se levanta la bandera para el cambio de password
                                //en su siguiente inicio de sesión
                                unLog.Info("INICIA Membership.GetUser()");
                                MembershipUser user = Membership.GetUser(txtUsuario.Text);
                                unLog.Info("TERMINA Membership.GetUser()");

                                if (user != null)
                                {
                                    user.Comment = "Se solicita cambio de contraseña por reseteo del usuario " + this.Usuario.ClaveUsuario;
                                    user.IsApproved = false;

                                    unLog.Info("INICIA Membership.UpdateUser()");
                                    Membership.UpdateUser(user);
                                    unLog.Info("TERMINA Membership.UpdateUser()");
                                }
                            }
                        }
                        catch
                        {
                            X.Msg.Alert("Actualizando Usuario", "Error al actualizar el usuario.").Show();
                            return;
                        }
                        finally
                        {
                            X.Msg.Notify("Actualizando Usuario", "Usuario actualizado <br />  <br /> <b> E X I T O S A M E N T E</b> <br />  <br /> ").Show();

                            FormPanelEditar.Collapsed = true;

                            PreparaGrid();
                            BindDataUsuarios();
                        }
                    }
                    else
                    {
                        X.Msg.Alert("Actualizando Usuario", "Las contraseñas no coinciden").Show();
                        return;
                    }
                }
                catch
                {
                    X.Msg.Alert("Actualizando Usuario", "Ocurrió un error al crear el usuario. Intenta más tarde.").Show();
                    return;
                }

            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Usuario", err.Mensaje()).Show();
            }
            catch (Exception err)
            {
                unLog.WarnException(err);
                X.Msg.Alert("Actualización de Usuario", "Ocurrió un error al modificar el usuario. Intenta más tarde.").Show();
                return;
            }
        }

        private void LimpiaCampos2()
        {
            txtAmaterno.Text = "";
            txtApaterno.Text = "";
            txtNombre.Text = "";
            txtEmail.Text = "";
            txtPass1.Text = "";
            txtPass2.Text = "";
            txtUsuario.Text = "";
        }

        public string GetErrorMessage(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "El usuario ya existe. Por favor, proporciona uno nuevo.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "El email proporcionado ya existe.";

                case MembershipCreateStatus.InvalidPassword:
                    return "La contraseña es inválida. Por favor, ingresa una nuevo de " + Membership.MinRequiredPasswordLength + " caracteres.";

                case MembershipCreateStatus.InvalidEmail:
                    return "El email es inválido.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Usuario inválido. Intenta de nuevo.";

                case MembershipCreateStatus.ProviderError:
                    return "Error genérico.";

                case MembershipCreateStatus.UserRejected:
                    return "Error de usuario denegado.";

                default:
                    return "Error desconocido. Por favor, contacta al Administrador.";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {
            FormPanelEditar.Collapsed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridUsuarios_DblClik(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_EditUsr);
            log.Info("EditarUsuarioDesdeCadenaComercial GridUsuarios_DblClik()");

            try
            {
                Guid unvalor = Guid.Parse(e.ExtraParams["UserId"]);
                String SelectedUser = (String)e.ExtraParams["UserName"];

                HttpContext.Current.Session["UserID"] = unvalor;
                
                //Abrir el Panel de Edicion.
                FormPanelEditar.Reset();
                FormPanelEditar.Collapsed = false;

                log.Info("INICIA ObtieneDatosEditarUsuario()");
                Usuario miUser = DAOUsuario.ObtieneDatosEditarUsuario(SelectedUser, LH_EditUsr);
                log.Info("TERMINA ObtieneDatosEditarUsuario()");

                miUser.AMaterno = (String)e.ExtraParams["Amaterno"];
                miUser.APaterno = (String)e.ExtraParams["Apaterno"];
                miUser.Nombre = (String)e.ExtraParams["Nombre"];

                txtAmaterno.Text = miUser.AMaterno;
                txtApaterno.Text = miUser.APaterno;
                txtNombre.Text = miUser.Nombre;
                txtEmail.Text = miUser.Email;
                txtUsuario.Text = miUser.ClaveUsuario;

                cmbTipoColectiva.SetValueAndFireSelect(miUser.ClaveColectiva);

                log.Info("INICIA ListaColectivaCadenasComercial()");
                SCColectiva.DataSource = DALAutorizador.BaseDatos.DAOCatalogos.ListaColectivaCadenasComercial(
                    (String)cmbTipoColectiva.Value, this.Usuario, 
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_EditUsr);
                log.Info("TERMINA ListaColectivaCadenasComercial()");
                SCColectiva.DataBind();

                cmbCadenaComercial2.SetValueAndFireSelect(miUser.ID_Colectiva);
                
                FormPanelEditar.Title = "Editando el Usuario: [" + SelectedUser + "]";

                //Llenar el grid de valores de campos de aplicaciones
                PreparaGrid();
                BindDataUsuarios();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Usuarios", err.Mensaje()).Show();
            }
            catch (Exception err)
            {
                log.WarnException(err);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            FormPanelEditar.Reset();
            FormPanelEditar.Collapsed = true;
        }

        private void AgregaRecordFiles()
        {
            SAllUser.AddField(new RecordField("UserId"));
            SAllUser.AddField(new RecordField("UserName"));
            SAllUser.AddField(new RecordField("Nombre"));
            SAllUser.AddField(new RecordField("Apaterno"));
            SAllUser.AddField(new RecordField("Amaterno"));
            SAllUser.AddField(new RecordField("Email"));
            SAllUser.AddField(new RecordField("IsLockedOut"));

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

            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Acciones";
            acciones.Width = 80;
            acciones.PrepareToolbar.Fn = "prepareToolbar";

            GridCommand Bloquear = new GridCommand();
            Bloquear.Icon = Icon.Lightbulb;
            Bloquear.CommandName = "Bloquear";
            Bloquear.ToolTip.Text = "Inactivar Usuario";
            acciones.Commands.Add(Bloquear);

            GridCommand play = new GridCommand();
            play.Icon = Icon.LightbulbOff;
            play.CommandName = "Activar";
            play.ToolTip.Text = "Activar Usuario";
            acciones.Commands.Add(play);

            GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel1.ColumnModel.Columns.Add(colID);
            GridPanel1.ColumnModel.Columns.Add(coluserName);
            GridPanel1.ColumnModel.Columns.Add(colNombre);
            GridPanel1.ColumnModel.Columns.Add(colAPaterno);
            GridPanel1.ColumnModel.Columns.Add(colAMaterno);
            GridPanel1.ColumnModel.Columns.Add(colEmail);

            ////AGREGAR EVENTOS
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("UserId", "this.getRowsValues({ selectedOnly: true })[0].UserId", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("UserName", "this.getRowsValues({ selectedOnly: true })[0].UserName", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("Apaterno", "this.getRowsValues({ selectedOnly: true })[0].Apaterno", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("Amaterno", "this.getRowsValues({ selectedOnly: true })[0].Amaterno", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("Nombre", "this.getRowsValues({ selectedOnly: true })[0].Nombre", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("UserId", "record.data.UserId", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("UserName", "record.data.UserName", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));

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

        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_EditUsr);
            unLog.Info("CentralUsuariosEditar  RefreshGrid()");

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

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_EditUsr);

            try
            {
                Guid ID_User = new Guid(e.ExtraParams["UserId"]);
                string EjecutarComando = e.ExtraParams["Comando"];
                string UserName = e.ExtraParams["UserName"];

                Usuario elUser = this.Usuario;

                switch (EjecutarComando)
                {
                    case "Bloquear":
                        logPCI.Info("INICIA BloquearUsuario()");
                        DAOUsuario.BloquearUsuario(UserName, LH_EditUsr, this.Usuario);
                        logPCI.Info("TERMINA BloquearUsuario()");
                        break;

                    case "Activar":
                        logPCI.Info("INICIA ActivarUsuario()");
                        LNUsuarios.ActivarUsuario(ID_User, LH_EditUsr, this.Usuario);
                        logPCI.Info("TERMINA ActivarUsuario()");
                        break;
                }

                PreparaGrid();
                BindDataUsuarios();

                X.Msg.Notify("Editar Usuario", "Comando ejecutado con <br />  <br /> <b> É X I T O </b> <br />  <br /> ").Show();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Editar Usuario", caEx.Mensaje()).Show();
            }
            catch (Exception err)
            {
                logPCI.ErrorException(err);
                X.Msg.Alert("Editar Usuario", "Ocurrió un error al ejecutar la acción seleccionada.").Show();
            }
        }
    }
}