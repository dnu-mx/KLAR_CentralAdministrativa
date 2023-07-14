using Autenticacion;
using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.LogicaNegocio;
using Ext.Net;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Web.Security;

namespace Usuarios
{
    public partial class CentralUsuariosNuevo : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER PARABILIA NEW USER
        private LogHeader LH_ParabNewUsr = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabNewUsr.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabNewUsr.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabNewUsr.User = this.Usuario.ClaveUsuario;
            LH_ParabNewUsr.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabNewUsr);
            string errRedirect = string.Empty;


            try
            {
                log.Info("INICIA CentralUsuariosNuevo Page_Load()");

                if (!IsPostBack)
                {
                    LlenaAplicaciones();

                    PreparaGrid();
                    BindDataUsuarios();
                }

                log.Info("TERMINA CentralUsuariosNuevo Page_Load()");
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

        private void BindDataUsuarios()
        {
            LogPCI elLog = new LogPCI(LH_ParabNewUsr);

            elLog.Info("INICIA ObtieneUsuarios()");
            GridPanel1.GetStore().DataSource = DAOCatalogos.ObtieneUsuarios((this.Usuario).UsuarioTemp, 
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabNewUsr);
            elLog.Info("TERMINA ObtieneUsuarios()");

            GridPanel1.GetStore().DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            LogPCI elLog = new LogPCI(LH_ParabNewUsr);

            try
            {
                Usuario nuevoUsuario = new Usuario();

                if (txtAmaterno.Text == "" ||
                   txtApaterno.Text == "" ||
                   txtNombre.Text == "" ||
                   txtEmail.Text == "" ||
                   txtPass1.Text == "" ||
                   txtPass2.Text == "" ||
                   txtUsuario.Text == "")
                {
                    X.Msg.Alert("Agregar Usuario", "Campos vacíos. Por favor, proporciona todos los campos.").Show();
                    return;
                }

                if (!Validaciones.ValidateIpSecurityParameters(this.chkBxValidateHashIpSecurity.Checked,
                    this.TextFieldIP.Text))
                {
                    X.Msg.Alert("Agregar Usuario", "Por favor, ingresa una Dirección IP válida.").Show();
                    return;
                }

                Password datosPassword = new Password();

                datosPassword.NombreUsuario = this.txtUsuario.Text;
                datosPassword.Nuevo = this.txtPass1.Text;
                datosPassword.Repeticion = this.txtPass2.Text;
                datosPassword.ExpresionRegular = ConfigurationManager.AppSettings["PasswordStrengthRegularExpression"].ToString();

                if (Validaciones.CondicionesPassword(datosPassword, false, LH_ParabNewUsr))
                {
                    string passwordHash = string.Empty;
                    string passwordSaltHash = string.Empty;
                    int iteraciones = 0;
                    string ipAdressHash = string.Empty;

                    string ip = this.TextFieldIP.Text;
                    if (!this.chkBxValidateHashIpSecurity.Checked)
                        ip = string.Empty;

                    Hashing.CreaPasswordUsuario(this.txtPass1.Text, ref passwordHash, ref passwordSaltHash,
                        ref iteraciones, ref ipAdressHash, LH_ParabNewUsr, ip, datosPassword.NombreUsuario);

                    datosPassword.Hash = passwordHash;
                    datosPassword.SaltHash = passwordSaltHash;
                    datosPassword.Iteraciones = iteraciones.ToString();
                    datosPassword.Email = this.txtEmail.Text;
                    datosPassword.StatusHashIPSecurity = (int)(this.chkBxValidateHashIpSecurity.Checked ? HashIpValidationsLifeCycle.ACTIVE : HashIpValidationsLifeCycle.NONE);
                    datosPassword.LocalIP = ipAdressHash;

                    ///Historial
                    string historyPwdHash = string.Empty;
                    string historyPwdSaltHash = string.Empty;
                    int historyIteraciones = 0;

                    Hashing.CreaPasswordHistorial(this.txtPass1.Text, datosPassword.NombreUsuario, ref historyPwdHash, 
                        ref historyPwdSaltHash, ref historyIteraciones, LH_ParabNewUsr);

                    datosPassword.HistoryHash = historyPwdHash;
                    datosPassword.HistorySaltHash = historyPwdSaltHash;
                    datosPassword.HistoryIteraciones = historyIteraciones.ToString();

                    //Datos personales del usuario
                    Usuario newUser = new Usuario();
                    newUser.Nombre = this.txtNombre.Text;
                    newUser.APaterno = this.txtApaterno.Text;
                    newUser.AMaterno = this.txtAmaterno.Text;
                    newUser.UserName = this.txtUsuario.Text;
                    newUser.ApplicationID = Guid.Parse(this.hdnAppId.Value.ToString());

                    elLog.Info("INICIA CreaNuevoUsuario()");
                    string UserId = LNUsuarios.CreaNuevoUsuario(datosPassword, newUser, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabNewUsr);
                    elLog.Info("TERMINA CreaNuevoUsuario()");

                    FormPanel1.Reset();
                    PreparaGrid();
                    BindDataUsuarios();

                    X.Msg.Alert("Agregar Usuario", "El Usuario se agregó correctamente.<br/><b>No olvides asignarle Roles y Permisos a Datos</b>").Show();
                }
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Agregar Usuario", caEx.Mensaje()).Show();
            }
            catch (Exception err)
            {
                elLog.WarnException(err);
                X.Msg.Alert("Agregar Usuario", "Ocurrió un error al crear el Usuario. Por favor, intenta más tarde.").Show();
                return;
            }
        }



        public string GetErrorMessage(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "El Usuario ya existe. Por favor, proporciona uno nuevo.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "El Correo Electrónico proporcionado ya existe.";

                case MembershipCreateStatus.InvalidPassword:
                    return "La Contraseña no cumple con los requisitos de seguridad.";

                case MembershipCreateStatus.InvalidEmail:
                    return "El Correo Electrónico es inválido.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "La respuesta de recuperación del Password es inválida. Por favor, verifica el valor e intenta nuevamente.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "La pregunta de recuperación de Password es inválida. Por favor, verifica el valor e intenta nuevamente.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Usuario inválido. Por favor, intenta de nuevo.";

                case MembershipCreateStatus.ProviderError:
                    return "Error del proveedor. Por favor, intenta de nuevo.";

                case MembershipCreateStatus.UserRejected:
                    return "Error. Usuario rechazado. Por favor, intenta de nuevo.";

                default:
                    return "Error desconocido. Por favor, contacta al Administrador del Sistema.";
            }
        }

        public void LlenaAplicaciones()
        {
            LogPCI unLog = new LogPCI(LH_ParabNewUsr);

            unLog.Info("INICIA ListaAplicaciones()");
            SCAplicaciones.DataSource = DAOCatalogos.ListaAplicaciones(LH_ParabNewUsr);
            unLog.Info("TERMINA ListaAplicaciones()");

            SCAplicaciones.DataBind();
        }
        
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Panel5.Collapsed = true;
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
            //LIMPIA GRID
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
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Parameter("UserId", "this.getRowsValues({ selectedOnly: true })[0].UserId", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Parameter("UserName", "this.getRowsValues({ selectedOnly: true })[0].UserName", ParameterMode.Raw));

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
            LogPCI logPCI = new LogPCI(LH_ParabNewUsr);
            logPCI.Info("CentralUsuariosNuevo RefreshGrid()");

            try
            {
                PreparaGrid();
                BindDataUsuarios();
            }
            catch (Exception err)
            {
                logPCI.WarnException(err);
            }
        }
    }
}
