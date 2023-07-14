using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using Ext.Net;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Web;


namespace Usuarios
{
    public partial class AdministrarRoles : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER PARABILIA EDIT USER
        private LogHeader LH_AdminRoles = new LogHeader();

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_AdminRoles.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_AdminRoles.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_AdminRoles.User = this.Usuario.ClaveUsuario;
            LH_AdminRoles.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_AdminRoles);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdministrarRoles Page_Load()");

                if (!IsPostBack)
                {
                    HttpContext.Current.Session["RoleId"] = Guid.Parse("7DA04EE9-7191-4F82-8ECC-C8F6C87A37D0");
                    BindPerfiles();   
                }

                log.Info("TERMINA AdministrarRoles Page_Load()");
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

        protected void btnGuardarConfig_Click(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_AdminRoles);

            try
            {
                unLog.Info("INICIA AgregarRol()");
                DAORol.AgregarRol(this.Usuario, txtRolName.Text, txtDescripcion.Text, LH_AdminRoles);
                unLog.Info("TERMINA AgregarRol()");

                FormPanel1.Reset();

                BindPerfiles();
            }
            catch (Exception err)
            {
                unLog.WarnException(err);
                X.Msg.Alert("Añadir Rol", "Ocurrió un error al añadir el rol.").Show();
            }
        }

        protected void PreparaGridRoles()
        {
           //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();
            string fn = "loadPage(#{Pages}, 'MenuEditor.aspx',#{GridPanel1}.getRowsValues({ selectedOnly: true })[0].RoleId,#{GridPanel1}.getRowsValues({ selectedOnly: true })[0].RoleName);";

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
            colEmail.Header = "Nombre Corto";
            colEmail.Sortable = true;
            colEmail.Width = 140;
            lasColumnas.Columns.Add(colEmail);

            Column colAPaterno = new Column();
            colAPaterno.DataIndex = "Description";
            colAPaterno.Header = "Descripcion";
            colAPaterno.Sortable = true;
            lasColumnas.Columns.Add(colAPaterno);

            GridPanel1.ColumnModel.Columns.Add(coluserName);
            GridPanel1.ColumnModel.Columns.Add(colNombre);
            GridPanel1.ColumnModel.Columns.Add(colEmail);
            GridPanel1.ColumnModel.Columns.Add(colAPaterno);

            GridPanel1.Listeners.DblClick.Handler = fn;
                
            GridFilters losFiltros = new GridFilters();

            StringFilter filAPaterno = new StringFilter();
            filAPaterno.DataIndex = "RoleName";
            losFiltros.Filters.Add(filAPaterno);

            StringFilter filAMaterno = new StringFilter();
            filAMaterno.DataIndex = "Description";
            losFiltros.Filters.Add(filAMaterno);

            GridPanel1.Plugins.Add(losFiltros);
        }


        private void BindPerfiles()
        {
            LogPCI unLog = new LogPCI(LH_AdminRoles);

            PreparaGridRoles();

            unLog.Info("INICIA ListaPerfiles()");
            SRoles.DataSource = DAOCatalogos.ListaPerfiles(LH_AdminRoles);
            unLog.Info("TERMINA ListaPerfiles()");

            SRoles.DataBind();
        }

    }
}