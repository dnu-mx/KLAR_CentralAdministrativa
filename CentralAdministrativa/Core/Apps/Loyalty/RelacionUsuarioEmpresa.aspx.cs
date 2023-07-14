using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using WebServices.Entidades;

namespace CentroContacto
{
    public partial class RelacionUsuarioEmpresa : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER RELACION USUARIO EMPRESA
        private LogHeader LH_RelUsuarioEmpresa = new LogHeader();

        #endregion

        protected class EmpresaComboPredictivo
        {
            public long ID_Colectiva { get; set; }
            public string ClaveColectiva { get; set; }
            public string NombreORazonSocial { get; set; }
            public string Cuenta { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_RelUsuarioEmpresa.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_RelUsuarioEmpresa.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_RelUsuarioEmpresa.User = this.Usuario.ClaveUsuario;
            LH_RelUsuarioEmpresa.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_RelUsuarioEmpresa);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA RelacionUsuarioEmpresa Page_Load()");

                if (!IsPostBack)
                {
                    EstableceComboEmpresas();

                    this.PagingUsuarios.PageSize =
                       Convert.ToInt32(Configuracion.Get(Guid.Parse(
                       ConfigurationManager.AppSettings["IDApplication"].ToString()),
                       "Reporte_RegsPorPagina", LH_RelUsuarioEmpresa).Valor);

                    HttpContext.Current.Session.Add("DtUsuarios", null);
                }

                log.Info("TERMINA RelacionUsuarioEmpresa Page_Load()");
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

        /// <summary>
        /// Establece los valores del combobox para empresas
        /// </summary>
        protected void EstableceComboEmpresas()
        {
            LogPCI unLog = new LogPCI(LH_RelUsuarioEmpresa);

            try
            {
                unLog.Info("INICIA ListaColectivasEmpresa()");
                DataTable dtEmpresas = DAOColectiva.ListaColectivasEmpresa(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_RelUsuarioEmpresa);
                unLog.Info("TERMINA ListaColectivasEmpresa()");

                List<EmpresaComboPredictivo> empresas = new List<EmpresaComboPredictivo>();

                foreach (DataRow empresa in dtEmpresas.Rows)
                {
                    var empresaCombo = new EmpresaComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt64(empresa["ID_Colectiva"].ToString()),
                        ClaveColectiva = empresa["ClaveColectiva"].ToString(),
                        NombreORazonSocial = empresa["NombreORazonSocial"].ToString(),
                        Cuenta = empresa["Cuenta"].ToString(),
                    };
                    empresas.Add(empresaCombo);
                }

                this.StoreEmpresa.DataSource = empresas;
                this.StoreEmpresa.DataBind();
            }
            catch (CAppException caEx)
            {
                unLog.Error(caEx.Mensaje());
                throw new Exception();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreUsuarios_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelUsuarios(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid, así como el ordenamiento y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelUsuarios(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_RelUsuarioEmpresa);

            try
            {
                DataTable dtUsuarios = new DataTable();

                dtUsuarios = HttpContext.Current.Session["DtUsuarios"] as DataTable;

                if (dtUsuarios == null)
                {
                    unLog.Info("INICIA ObtieneUsuariosAppConnect()");
                    dtUsuarios = DAOReportes.ObtieneUsuariosAppConnect(LH_RelUsuarioEmpresa);
                    unLog.Info("TERMINA ObtieneUsuariosAppConnect()");

                    HttpContext.Current.Session.Add("DtUsuarios", dtUsuarios);
                }

                int TotalRegistros = dtUsuarios.Rows.Count;

                (this.StoreUsuarios.Proxy[0] as PageProxy).Total = TotalRegistros;

                DataTable sortedDT = dtUsuarios.Clone();
                DataTable dtToGrid = dtUsuarios.Clone();

                //Se ordenan los datos según la elección del usuario
                if (!String.IsNullOrEmpty(Columna))
                {
                    System.Data.DataView dv = dtUsuarios.DefaultView;

                    dv.Sort = Columna + " " + Orden.ToString();
                    sortedDT = dv.ToTable();
                    sortedDT.AcceptChanges();
                }

                int RegistroFinal = (RegistroInicial + PagingUsuarios.PageSize) < TotalRegistros ?
                    (RegistroInicial + PagingUsuarios.PageSize) : TotalRegistros;

                //Se recorta el número de registros a los definidos por página
                for (int row = RegistroInicial; row < RegistroFinal; row++)
                {
                    dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtUsuarios.Rows[row] : sortedDT.Rows[row]);
                }

                dtToGrid.AcceptChanges();

                this.StoreUsuarios.DataSource = dtToGrid;
                this.StoreUsuarios.DataBind();

            }

            catch (CAppException err)
            {
                X.Msg.Alert("Usuarios", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Usuarios", "Ocurrió un error al obtener los usuarios.").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento Quitar Selección a una fila del grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {
            LimpiaPanelEste();
        }

        /// <summary>
        /// Controla el evento Doble Click a una fila del grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void GridPanelUsuarios_DblClik(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_RelUsuarioEmpresa);

            try
            {
                LimpiaPanelEste();

                char[] charsToTrim = { '*', '"', ' ' };
                string Usuario = e.ExtraParams["Usuario"].Trim(charsToTrim);
                this.hdnIdUsuario.Value = e.ExtraParams["ID_Usuario"].Trim(charsToTrim);

                unLog.Info("INICIA LoginWebService()");
                Parametros.Headers losHeaders = LNAppConnect.LoginWebService(LH_RelUsuarioEmpresa);
                unLog.Info("TERMINA LoginWebService()");

                Parametros.ConsultarUsuariosEmpresaBody elBody = new Parametros.ConsultarUsuariosEmpresaBody
                {
                    idUsuario = this.hdnIdUsuario.Value.ToString()
                };

                unLog.Info("INICIA ConsultaUsuarioEmpresa()");
                this.lblRelacionActual.Text = LNAppConnect.ConsultaUsuarioEmpresa(losHeaders, elBody, LH_RelUsuarioEmpresa);
                unLog.Info("TERMINA ConsultaUsuarioEmpresa()");

                this.EastFormPanel.Title += Usuario.Trim().ToUpper() == "NULL" ? "" : Usuario;
                this.EastFormPanel.Collapsed = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Empresa Relacionada", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Empresa Relacionada", "Ocurrió un error al seleccionar el Usuario").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Aceptar del panel Relacionar Otra Empresa, solicitando la actualización
        /// de la relación al servicio web
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnActualizaEmpresa_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_RelUsuarioEmpresa);

            try
            {
                log.Info("INICIA LoginWebService()");
                Parametros.Headers losHeaders = LNAppConnect.LoginWebService(LH_RelUsuarioEmpresa);
                log.Info("TERMINA LoginWebService()");

                Parametros.UsuariosEmpresaBody bodyActualizar = new Parametros.UsuariosEmpresaBody();
                bodyActualizar.idUsuario = this.hdnIdUsuario.Value.ToString();
                bodyActualizar.ClaveEmpresa = this.hdnClaveEmpresa.Value.ToString();
                bodyActualizar.NombreEmpresa = this.hdnNombreEmpresa.Value.ToString();
                bodyActualizar.CuentaEmpresa = this.hdnCuentaEmpresa.Value.ToString();

                log.Info("INICIA ActualizaUsuarioEmpresa()");
                LNAppConnect.ActualizaUsuarioEmpresa(losHeaders, bodyActualizar, LH_RelUsuarioEmpresa);
                log.Info("TERMINA ActualizaUsuarioEmpresa()");

                X.Msg.Notify("", "Relación de Usuario/Empresa" + "<br />  <br /> <b> E X I T O S A </b> <br />  <br /> ").Show();

                Parametros.ConsultarUsuariosEmpresaBody bodyConsultar = new Parametros.ConsultarUsuariosEmpresaBody();
                bodyConsultar.idUsuario = this.hdnIdUsuario.Value.ToString();

                log.Info("INICIA ConsultaUsuarioEmpresa()");
                this.lblRelacionActual.Text = LNAppConnect.ConsultaUsuarioEmpresa(losHeaders, bodyConsultar, LH_RelUsuarioEmpresa);
                log.Info("TERMINA ConsultaUsuarioEmpresa()");
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Relacionar Empresa", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Relacionar Empresa", "Ocurrió un error al relacionar la Empresa con el Usuario").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Cancelar del panel Relacionar Otra Empresa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiaPanelEste();
        }

        /// <summary>
        /// Restablece los controles relacionados al panel este a su estado de carga inicial
        /// </summary>
        protected void LimpiaPanelEste()
        {
            this.lblRelacionActual.Text = string.Empty;
            this.cBoxEmpresa.Reset();
            this.EastFormPanel.Title = "Usuario - ";
            this.EastFormPanel.Collapse();
        }
    }
}