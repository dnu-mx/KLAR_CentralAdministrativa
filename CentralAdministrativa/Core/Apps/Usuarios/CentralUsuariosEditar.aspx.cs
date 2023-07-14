using Autenticacion;
using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Data;
using System.Web;

namespace Usuarios
{
    public partial class CentralUsuariosEditar : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER PARABILIA EDIT USER
        private LogHeader LH_ParabEditUsr = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabEditUsr.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabEditUsr.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabEditUsr.User = this.Usuario.ClaveUsuario;
            LH_ParabEditUsr.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabEditUsr);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA CentralUsuariosEditar Page_Load()");

                if (!IsPostBack)
                {
                    LlenaAplicaciones();

                    PreparaGrid();
                    BindDataUsuarios();

                    HttpContext.Current.Session.Add("_SLT", null);
                    HttpContext.Current.Session.Add("_IT", null);
                }

                log.Info("TERMINA  CentralUsuariosEditar Page_Load()");
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
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

        public void LlenaAplicaciones()
        {
            LogPCI unLog = new LogPCI(LH_ParabEditUsr);

            unLog.Info("INICIA ListaAplicaciones()");
            SCAplicaciones.DataSource = DAOCatalogos.ListaAplicaciones(LH_ParabEditUsr);
            unLog.Info("TERMINA ListaAplicaciones()");

            SCAplicaciones.DataBind();
        }


        private void BindDataUsuarios()
        {
            LogPCI unLog = new LogPCI(LH_ParabEditUsr);

            unLog.Info("INICIA ObtieneUsuarios()");
            GridPanel1.GetStore().DataSource = DAOCatalogos.ObtieneUsuarios((this.Usuario).UsuarioTemp,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabEditUsr);
            unLog.Info("TERMINA ObtieneUsuarios()");

            GridPanel1.GetStore().DataBind();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabEditUsr);

            try
            {
                if (txtAmaterno.Text == "" ||
                   txtApaterno.Text == "" ||
                   txtNombre.Text == "" ||
                   txtEmail.Text == "" ||
                   txtUsuario.Text == "")
                {
                    X.Msg.Alert("Actualizar Usuario", "Campos vacíos. Por favor, proporciona todos los campos.").Show();
                    return;
                }

                int changeIpStatus = GetHashValidationNewValue(Convert.ToInt32(this.hdnCurrentStatusIP.Value), this.chkBxValidateHashIpSecurity.Checked);

                unLog.Info("INICIA ObtieneDatosAutenticacionUsuario()");
                Password losDatos = DAOUsuario.ObtieneDatosAutenticacionUsuario(this.txtUsuario.Text, LH_ParabEditUsr);
                unLog.Info("TERMINA ObtieneDatosAutenticacionUsuario()");
                
                HttpContext.Current.Session.Add("_SLT", losDatos.SaltHash);
                HttpContext.Current.Session.Add("_IT", losDatos.Iteraciones);

                bool msjValidacion = true;
                string _hashIP = string.Empty;
                string _saltIP = string.Empty;
                string _itIP = string.Empty;

                switch (changeIpStatus)
                {
                    case (int)HashIpValidationsLifeCycle.NONE:
                        this.hdnNewIP.Value = this.txtIP.Text;
                        msjValidacion = false;
                        EditaDatosUsuario();
                        break;

                    case (int)HashIpValidationsLifeCycle.NONE_TO_ACTIVE:
                        if (!Validaciones.ValidateIpSecurityParameters(this.chkBxValidateHashIpSecurity.Checked, this.txtIP.Text))
                        {
                            X.Msg.Alert("Actualizar Usuario", "Por favor, ingresa una Dirección IP válida.").Show();
                            return;
                        }

                        Validaciones.NuevaDireccionIP(this.txtIP.Text, ref _hashIP, losDatos.SaltHash, ref _saltIP,
                            losDatos.Iteraciones, ref _itIP, LH_ParabEditUsr);

                        this.hdnNewIP.Value = _hashIP;
                        HttpContext.Current.Session.Add("_SLT", _saltIP);
                        HttpContext.Current.Session.Add("_IT", _itIP);
                        break;

                    case (int)HashIpValidationsLifeCycle.ACTIVE:
                        if (this.txtIP.Text != this.hdnCurrentIP.Value.ToString().Substring(0, 15))
                        {
                            if (!Validaciones.ValidateIpSecurityParameters(this.chkBxValidateHashIpSecurity.Checked, this.txtIP.Text))
                            {
                                X.Msg.Alert("Actualizar Usuario", "Por favor, ingresa una Dirección IP válida.").Show();
                                return;
                            }

                            Validaciones.NuevaDireccionIP(this.txtIP.Text, ref _hashIP, losDatos.SaltHash, ref _saltIP,
                                losDatos.Iteraciones, ref _itIP, LH_ParabEditUsr);

                            this.hdnNewIP.Value = _hashIP;
                            HttpContext.Current.Session.Add("_SLT", _saltIP);
                            HttpContext.Current.Session.Add("_IT", _itIP);
                            break;
                        }
                        else
                        {
                            this.hdnNewIP.Value = this.hdnCurrentIP.Value;
                            msjValidacion = false;
                            EditaDatosUsuario();
                        }
                        break;

                    //case (int)HashIpValidationsLifeCycle.ACTIVE_TO_NONE:
                    default:
                        this.hdnNewIP.Value = this.txtIP.Text;
                        break;
                }

                if (msjValidacion)
                    this.btnValidaModNodo.FireEvent("click");
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Actualizar Usuario", caEx.Mensaje()).Show();
            }
            catch (Exception err)
            {
                unLog.WarnException(err);
                X.Msg.Alert("Actualizar Usuario", "Ocurrió un error al actualizar el Usuario. Por favor, intenta más tarde.").Show();
                return;
            }
        }


        /// <summary>
        /// Método que establece el valor de los controles relacionados con el Nodo IP, tras la confirmación o no
        /// del usuario por modificar el checkbox del Nodo IP
        /// </summary>
        /// <param name="Confirmado">Bandera de confirmación del usuario</param>
        [DirectMethod(Namespace = "EditaUsuarios")]
        public void ModificaNodo(bool Confirmado)
        {
            if (!Confirmado)
            {
                this.chkBxRstPwd.Checked = Convert.ToBoolean(this.hdnCheckReset.Value);
                this.chkBxValidateHashIpSecurity.Checked = 
                    (Convert.ToInt32(this.hdnCurrentStatusIP.Value) == (int)HashIpValidationsLifeCycle.ACTIVE);
                this.txtIP.Text = string.IsNullOrEmpty(this.hdnCurrentIP.Value.ToString()) ?
                    this.hdnCurrentIP.Value.ToString() : this.hdnCurrentIP.Value.ToString().Substring(0, 15);
            }
            else
            {
                this.chkBxRstPwd.Checked = Confirmado;
                EditaDatosUsuario();
            }
        }


        protected void EditaDatosUsuario()
        {
            LogPCI logPCI = new LogPCI(LH_ParabEditUsr);

            try
            {
                logPCI.Info("INICIA ObtieneCaracteristicasUsuario()");
                Usuario nuevoUsuario = DAOUsuario.ObtieneCaracteristicasUsuario(txtUsuario.Text, LH_ParabEditUsr);
                logPCI.Info("TERMINA ObtieneCaracteristicasUsuario()");

                nuevoUsuario.AMaterno = this.txtAmaterno.Text;
                nuevoUsuario.APaterno = this.txtApaterno.Text;
                nuevoUsuario.Nombre = this.txtNombre.Text;
                nuevoUsuario.Email = this.txtEmail.Text;
                nuevoUsuario.ApplicationID = Guid.Parse(this.hdnAppId.Value.ToString());
                nuevoUsuario.StatusHashIPSecurity = GetHashValidationNewValue(Convert.ToInt32(this.hdnCurrentStatusIP.Value), this.chkBxValidateHashIpSecurity.Checked);
                nuevoUsuario.LocalIP = this.hdnNewIP.Value.ToString();
                nuevoUsuario.ClaveUsuario = this.Usuario.ClaveUsuario;

                string _Salt = HttpContext.Current.Session["_SLT"].ToString();
                string _Itera = HttpContext.Current.Session["_IT"].ToString();

                logPCI.Info("INICIA ModificarUsuario()");
                LNUsuarios.ModificarUsuario(nuevoUsuario, _Salt, _Itera, "_Editar", LH_ParabEditUsr);
                logPCI.Info("TERMINA ModificarUsuario()");

                if (this.chkBxRstPwd.Checked)
                {
                    logPCI.Info("INICIA ReseteaPasswordUsuario()");
                    LNUsuarios.ReseteaPasswordUsuario(this.txtUsuario.Text, this.txtEmail.Text,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabEditUsr);
                    logPCI.Info("TERMINA ReseteaPasswordUsuario()");

                    X.Msg.Notify("Actualizar Usuario", "Correo enviado <br />  <br /> <b> E X I T O S A M E N T E</b> <br />  <br /> ").Show();
                }

                X.Msg.Notify("Actualizar Usuario", "Usuario actualizado <br />  <br /> <b> E X I T O S A M E N T E</b> <br />  <br /> ").Show();

                FormPanelEditar.Collapsed = true;

                PreparaGrid();
                BindDataUsuarios();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Actualizar Usuario", caEx.Mensaje()).Show();
            }
            catch (Exception err)
            {
                logPCI.WarnException(err);
                X.Msg.Alert("Actualizar Usuario", "Ocurrió un error al actualizar el Usuario. Por favor, intenta más tarde.").Show();
                return;
            }
        }

        private int GetHashValidationNewValue(int current, bool @checked)
        {
            if ((current == (int)HashIpValidationsLifeCycle.NONE) && @checked)
               return (int)HashIpValidationsLifeCycle.NONE_TO_ACTIVE;

            if ((current == (int)HashIpValidationsLifeCycle.ACTIVE) && !@checked)
                return (int)HashIpValidationsLifeCycle.ACTIVE_TO_NONE;

            return current;
        }


        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {
            FormPanelEditar.Collapsed = true;
        }


        protected void GridUsuarios_DblClik(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabEditUsr);
            unLog.Info("CentralUsuariosEditar GridUsuarios_DblClik()");

            try
            {
                Guid unvalor = Guid.Parse(e.ExtraParams["UserId"]);
                String SelectedUser = (String)e.ExtraParams["UserName"];

                HttpContext.Current.Session["UserID"] = unvalor;

                //Abrir el Panel de Edicion.
                FormPanelEditar.Reset();
                FormPanelEditar.Collapsed = false;

                unLog.Info("INICIA ObtieneDatosEditarUsuario()");
                Usuario miUser = DAOUsuario.ObtieneDatosEditarUsuario(SelectedUser, LH_ParabEditUsr);
                unLog.Info("TERMINA ObtieneDatosEditarUsuario()");
                
                miUser.AMaterno = e.ExtraParams["Amaterno"];
                miUser.APaterno = e.ExtraParams["Apaterno"];
                miUser.Nombre = e.ExtraParams["Nombre"];
                
                this.txtAmaterno.Text = miUser.AMaterno;
                this.txtApaterno.Text = miUser.APaterno;
                this.txtNombre.Text = miUser.Nombre;
                this.txtEmail.Text = miUser.Email;
                this.txtUsuario.Text = miUser.ClaveUsuario;
                this.chkBxValidateHashIpSecurity.Checked = (miUser.StatusHashIPSecurity == (int)HashIpValidationsLifeCycle.ACTIVE);
                this.hdnCurrentStatusIP.Value = miUser.StatusHashIPSecurity;
                this.txtIP.Text = string.IsNullOrEmpty(miUser.LocalIP) ? miUser.LocalIP : miUser.LocalIP.Substring(0, 15);
                this.hdnCurrentIP.Value = miUser.LocalIP;
                this.hdnCheckReset.Value = false;

                unLog.Info("INICIA ListaAplicaciones()");
                DataSet lasApps = DAOCatalogos.ListaAplicaciones(LH_ParabEditUsr);
                unLog.Info("TERMINA ListaAplicaciones()");

                SCAplicaciones.DataSource = lasApps;
                SCAplicaciones.DataBind();

                this.cmbAplicacion.SetValue(miUser.ApplicationName);
                DataRow[] laApp = lasApps.Tables[0].Select(string.Format("{0} = '{1}'", "ApplicationName", miUser.ApplicationName));
                if (laApp.Length == 1)
                {
                    this.hdnAppId.Value = laApp[0].ItemArray[0].ToString();
                }

                FormPanelEditar.Title = "Editando el Usuario: [" + SelectedUser + "]";

                //Llenar el grid de valores de campos de aplicaciones
                PreparaGrid();
                BindDataUsuarios();
            }
            catch (Exception err)
            {
                unLog.WarnException(err);
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
            coluserName.Width = 200;
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
            colEmail.Width = 250;
            lasColumnas.Columns.Add(colEmail);

            CommandColumn acciones = new CommandColumn();
            acciones.Header = "Estatus";
            acciones.Width = 50;
            acciones.PrepareToolbar.Fn = "prepareToolbar";

            GridCommand Desactivar = new GridCommand();
            Desactivar.Icon = Icon.RecordGreen;
            Desactivar.CommandName = "Desactivar";
            Desactivar.ToolTip.Text = "Inactivar Usuario";
            acciones.Commands.Add(Desactivar);

            GridCommand play = new GridCommand();
            play.Icon = Icon.RecordRed;
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
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Parameter("UserId", "this.getRowsValues({ selectedOnly: true })[0].UserId", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Parameter("UserName", "this.getRowsValues({ selectedOnly: true })[0].UserName", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Parameter("Apaterno", "this.getRowsValues({ selectedOnly: true })[0].Apaterno", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Parameter("Amaterno", "this.getRowsValues({ selectedOnly: true })[0].Amaterno", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Parameter("Nombre", "this.getRowsValues({ selectedOnly: true })[0].Nombre", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Parameter("UserId", "record.data.UserId", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Parameter("UserName", "record.data.UserName", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Parameter("Comando", "command", ParameterMode.Raw));

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
            LogPCI log = new LogPCI(LH_ParabEditUsr);
            log.Info("CentralUsuariosEditar  RefreshGrid()");

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
            LogPCI logPCI = new LogPCI(LH_ParabEditUsr);

            try
            {
                Guid ID_User = new Guid(e.ExtraParams["UserId"]);
                string EjecutarComando = e.ExtraParams["Comando"];
                string UserName = e.ExtraParams["UserName"];
             
                Usuario elUser = this.Usuario;

                switch (EjecutarComando)
                {
                    case "Desactivar":
                        logPCI.Info("INICIA DesactivarUsuario()");
                        LNUsuarios.DesactivarUsuario(UserName, LH_ParabEditUsr, this.Usuario);
                        logPCI.Info("TERMINA DesactivarUsuario()");
                        break;

                    case "Activar":
                        logPCI.Info("INICIA ActivarUsuario()");
                        LNUsuarios.ActivarUsuario(ID_User, LH_ParabEditUsr, this.Usuario);
                        logPCI.Info("TERMINA ActivarUsuario()");
                        break;
                }

                PreparaGrid();
                BindDataUsuarios();

                X.Msg.Notify("Los Usuarios", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Los Usuarios", caEx.Mensaje() + " Cod.(" + caEx.CodigoError().ToString() + ")").Show();
            }
            catch (Exception err)
            {
                logPCI.ErrorException(err);
                X.Msg.Alert("Los Usuarios", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }
    }
}