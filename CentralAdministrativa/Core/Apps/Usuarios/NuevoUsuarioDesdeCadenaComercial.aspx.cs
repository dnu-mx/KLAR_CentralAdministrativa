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
using System.Web.Security;

namespace Usuarios
{
    public partial class NuevoUsuarioDesdeCadenaComercial : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER NEW USER
        private LogHeader LH_NewUsr = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_NewUsr.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_NewUsr.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_NewUsr.User = this.Usuario.ClaveUsuario;
            LH_NewUsr.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_NewUsr);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA NuevoUsuarioDesdeCadenaComercial Page_Load()");

                if (!IsPostBack)
                {
                    LlenaTiposColectiva();
                }

                if (!IsPostBack)
                {
                    PreparaGrid();
                    BindDataUsuarios();
                }

                log.Info("TERMINA NuevoUsuarioDesdeCadenaComercial Page_Load()");
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
            LogPCI logPCI = new LogPCI(LH_NewUsr);

            logPCI.Info("INICIA ObtieneUsuarios()");
            GridPanel1.GetStore().DataSource = DAOCatalogos.ObtieneUsuarios(this.Usuario.UsuarioTemp,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_NewUsr);
            logPCI.Info("TERMINA ObtieneUsuarios()");

            GridPanel1.GetStore().DataBind();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_NewUsr);

            try
            {
                MembershipCreateStatus status;
                Usuario nuevoUsuario = new Usuario();

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
                            X.Msg.Alert("Status", "Campos Vacios, Por Favor proporciona todos los campos").Show();
                            return;
                        }

                        unLog.Info("INICIA Membership.CreateUser()");
                        MembershipUser newUser = Membership.CreateUser(txtUsuario.Text, txtPass1.Text,
                                                                       txtEmail.Text, "Pregunta",
                                                                       "Respuesta", true, out status);
                        unLog.Info("TERMINA Membership.CreateUser()");

                        if (newUser == null)
                        {
                            string elError = GetErrorMessage(status);
                            unLog.Warn("Usuario no creado. Membership.CreateUser status: " + elError);
                            X.Msg.Alert("Status", elError).Show();
                            return;
                        }
                        else
                        {
                            unLog.Info("INICIA ObtieneCaracteristicasUsuario()");
                            nuevoUsuario = DAOUsuario.ObtieneCaracteristicasUsuario(txtUsuario.Text, LH_NewUsr);
                            unLog.Info("TERMINA ObtieneCaracteristicasUsuario()");

                            nuevoUsuario.AMaterno = txtAmaterno.Text;
                            nuevoUsuario.APaterno = txtApaterno.Text;
                            nuevoUsuario.Nombre = txtNombre.Text;
                            nuevoUsuario.Email = txtEmail.Text;
                            nuevoUsuario.Password = txtPass1.Text;
                            nuevoUsuario.ID_Colectiva = Int64.Parse((String)cmbColectiva.Value);

                            unLog.Info("INICIA AgregarUsuario()");
                            LNUsuarios.AgregarUsuario(nuevoUsuario.ClaveColectiva, this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                                LH_NewUsr);
                            unLog.Info("TERMINA AgregarUsuario()");

                            FormPanel1.Reset();
                            PreparaGrid();
                            BindDataUsuarios();
                        }
                    }
                    else
                    {
                        X.Msg.Alert("Status", "Los Password no coinciden").Show();
                        return;
                    }
                }
                catch
                {
                    X.Msg.Alert("Status", "Ocurrio Un Error al crear el Usuario Intentalo más tarde").Show();
                    return;
                }

                X.Msg.Alert("Usuarios", "El Usuario se Agregó Correctamente,<br/><b>No olvides Asignarle Roles y Permisos a Datos</b>").Show();
                return;

            }
            catch (Exception err)
            {
                unLog.WarnException(err);
                X.Msg.Alert("Status", "Ocurrio Un Error al crear el Usuario Intentalo más tarde").Show();
                return;
            }

        }

        public string GetErrorMessage(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "EL USUARIO YA EXISTE, POR FAVOR PROPORCIONA UN NUEVO USUARIO";

                case MembershipCreateStatus.DuplicateEmail:
                    return "El Email proporcionado ya existe";

                case MembershipCreateStatus.InvalidPassword:
                    return "el Password es invalido, por favor ingresa uno nuevo de " + Membership.MinRequiredPasswordLength + " caracteres ";

                case MembershipCreateStatus.InvalidEmail:
                    return "El Email no es un valor valido";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Usuario invalido, Trata de Nuevo";

                case MembershipCreateStatus.ProviderError:
                    return "Error Generico";

                case MembershipCreateStatus.UserRejected:
                    return "Error de Usuario Denegado";

                default:
                    return "Error desconocido, por favor contacta al Administrador";
            }
        }

        public void LlenaTiposColectiva()
        {
            LogPCI log = new LogPCI(LH_NewUsr);

            log.Info("INICIA ListaTipoColectivaFiltroNivelCadena()");
            SCTipoColectiva.DataSource = DALAutorizador.BaseDatos.DAOCatalogos.ListaTipoColectivaFiltroNivelCadena(
                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_NewUsr);
            log.Info("TERMINA ListaTipoColectivaFiltroNivelCadena()");

            SCTipoColectiva.DataBind();
        }

        protected void TipoColectiva_Select(object sender, DirectEventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_NewUsr);

            try
            {
                cmbColectiva.Clear();

                logPCI.Info("INICIA ListaColectivaCadenasComercial()");
                SCColectiva.DataSource = DALAutorizador.BaseDatos.DAOCatalogos.ListaColectivaCadenasComercial(
                    (String)cmbTipoColectiva.Value, this.Usuario, 
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                logPCI.Info("TERMINA ListaColectivaCadenasComercial()");

                SCColectiva.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Colectivas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Colectivas", "Ocurrió un error al consultar las Colectivas").Show();
            }

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

        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_NewUsr);
            unLog.Info("NuevoUsuarioDesdeCadenaComercial RefreshGrid()");

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
    }
}
