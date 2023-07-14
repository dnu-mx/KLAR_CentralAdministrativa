using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALCentralAplicaciones;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;

namespace Administracion
{
    public partial class AdminInfoOnBoarding : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Administrar Parámetros
        private LogHeader LH_ParabAdminInfoOnBoarding = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Administrar Parámetros de OnBoarding 
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabAdminInfoOnBoarding.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabAdminInfoOnBoarding.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabAdminInfoOnBoarding.User = this.Usuario.ClaveUsuario;
            LH_ParabAdminInfoOnBoarding.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabAdminInfoOnBoarding);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdminInfoOnBoarding Page_Load()");

                if (!IsPostBack)
                {
                    LlenaGridResultados();
                }

                log.Info("TERMINA AdminInfoOnBoarding Page_Load()");
            }

            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
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

        /// <summary>
        /// Controla el evento Click al botón Aceptar de la ventana de Nuevo Producto
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnNuevoNodo_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminInfoOnBoarding);

            try
            {
                Nodo nodo = new Nodo();
                nodo.NombreKey = this.txtNombreKeyNuevo.Text;
                nodo.NombreNodoPadre = this.txtNombreNodoNuevo.Text;

                ParametroNodo parametro = new ParametroNodo();
                parametro.ValorKey = this.txtKeyNuevoParametroDefault.Text;
                parametro.DescripcionValor = this.txtDescNuevoParametroDefault.Text;

                pCI.Info("INICIA CreaNuevoNodo()");
                DataTable dt = LNInfoOnBoarding.CreaNuevoNodo(nodo, parametro, this.Usuario, LH_ParabAdminInfoOnBoarding);
                pCI.Info("INICIA CreaNuevoNodo()");

                string msj = dt.Rows[0]["Mensaje"].ToString();
                string idNodoNuevo = dt.Rows[0]["ID_NuevoNodo"].ToString();

                if (idNodoNuevo == "-1")
                {
                    X.Msg.Alert("Nuevo Nodo", msj).Show();
                }
                else
                {
                    this.WdwNuevoNodo.Hide();
                    this.txtBusquedaNodo.Text = nodo.NombreKey;
                    LlenaGridResultados();

                    X.Msg.Alert("Nuevo Nodo", "<br />" + msj + "<br /> <b> E X I T O S A M E N T E </b> <br /> ",
                        new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "Nodos.CargaNuevoNodo()",
                                Text = "Aceptar"
                            }
                        }).Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Nodo", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Nuevo Nodo", "Ocurrió un error al generar el Nuevo Nodo").Show();
            }
        }


        /// <summary>
        /// Controla el evento clic al botón Aceptar al mensaje informativo
        /// de creación exitosa de producto
        /// </summary>
        [DirectMethod(Namespace = "Nodos")]
        public void CargaNuevoNodo()
        {
            RowSelectionModel rsm = GridResultadosNodos.GetSelectionModel() as RowSelectionModel;
            rsm.SelectedRows.Add(new SelectedRow(0));
            rsm.UpdateSelection();

            GridResultadosNodos.FireEvent("RowClick");
        }

        /// <summary>
        /// Llena el grid de resultados de productos con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            LogPCI log = new LogPCI(LH_ParabAdminInfoOnBoarding);

            try
            {
                log.Info("INICIA ObtieneNodosPorClaveDesc()");
                DataSet dsNodos = DAOInfoOnBoarding.ObtieneNodosPorClaveDesc(String.IsNullOrEmpty(this.txtBusquedaNodo.Text) ? null : this.txtBusquedaNodo.Text,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabAdminInfoOnBoarding);
                log.Info("TERMINA ObtieneNodosPorClaveDesc()");

                if (dsNodos.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Nodos", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreNodos.DataSource = dsNodos;
                    StoreNodos.DataBind();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Nodos", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Nodos", "Ocurrió un error al obtener los Nodos").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel izquierdo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenaGridResultados();
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowResultadosPP_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdNodo = 0;
                string NombreKey = "", NombreNodoPadre = "";

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] nodo = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in nodo[0])
                {
                    switch (column.Key)
                    {
                        case "ID_ScoreNodo": IdNodo = int.Parse(column.Value); break;
                        case "NombreKey": NombreKey = column.Value; break;
                        case "NombreNodoPadre": NombreNodoPadre = column.Value; break;
                        default:
                            break;
                    }
                }

                this.hdnIdNodo.Value = IdNodo;

                PanelCentralNodos.Disabled = true;
                PanelCentralNodos.Title = NombreKey +  " - " + NombreNodoPadre;
                PanelCentralNodos.Disabled = false;

                txtNombreKey.Text = NombreKey;
                txtNombreNodo.Text = NombreNodoPadre;
                LlenaGridParametros(IdNodo);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nodos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ParabAdminInfoOnBoarding);
                unLog.ErrorException(ex);
                X.Msg.Alert("Nodos", "Ocurrió un error al obtener la información del Nodo").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del panel de Información Adicional 
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarInfoAd_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminInfoOnBoarding);

            try
            {
                Nodo unNodo = new Nodo();
                int idNodoSeleccionado = int.Parse(this.hdnIdNodo.Text);

                unNodo.ID_ScoreNodo = idNodoSeleccionado;
                unNodo.NombreKey = this.txtNombreKey.Text;
                unNodo.NombreNodoPadre = this.txtNombreNodo.Text;
                unNodo.EsActivo = true;

                log.Info("INICIA ModificaNodo()");
                string msjResp = LNInfoOnBoarding.ModificaNodo(unNodo, this.Usuario, LH_ParabAdminInfoOnBoarding);
                log.Info("TERMINA ModificaNodo()");

                if (!msjResp.Contains("OK"))
                {
                    X.Msg.Alert("Actualización de Nodo", msjResp).Show();
                }
                else
                {
                    X.Msg.Notify("", "Nodo Actualizado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                    LlenaGridResultados();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Nodo", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Actualización de Nodo", "Ocurrió un error al modificar el Nodo").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Eliminar Nodo del panel de Información Adicional 
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        [DirectMethod(Namespace = "AdminObjeto")]
        public void EliminarNodo()
        {
            LogPCI log = new LogPCI(LH_ParabAdminInfoOnBoarding);

            try
            {
                Nodo unNodo = new Nodo();
                int idNodoSeleccionado = int.Parse(this.hdnIdNodo.Text);

                unNodo.ID_ScoreNodo = idNodoSeleccionado;
                unNodo.NombreKey = "";
                unNodo.NombreNodoPadre = "";
                unNodo.EsActivo = false;

                log.Info("INICIA EliminaNodo()");
                string msjResp = LNInfoOnBoarding.ModificaNodo(unNodo, this.Usuario, LH_ParabAdminInfoOnBoarding);
                log.Info("TERMINA EliminaNodo()");

                Thread.Sleep(200);
                X.Mask.Hide();

                if (!msjResp.Contains("OK"))
                {
                    X.Msg.Alert("Eliminación de Nodo", msjResp).Show();
                }
                else
                {
                    X.Msg.Notify("", "Nodo Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                    btnLimpiarIzq_Click(null, null);
                    LlenaGridResultados();
                }
            }
            catch (CAppException err)
            {
                Thread.Sleep(200);
                X.Mask.Hide();
                X.Msg.Alert("Eliminación de Nodo", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Thread.Sleep(200);
                X.Mask.Hide();

                log.ErrorException(ex);
                X.Msg.Alert("Eliminación de Nodo", "Ocurrió un error al eliminar el Nodo").Show();
            }
        }

        /// <summary>
        /// Llena el grid de Parametros con la información consultada a base de datos
        /// </summary>
        /// <param name="IdNodo">Identificador del nodo</param>
        protected void LlenaGridParametros(int IdNodo)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminInfoOnBoarding);

            try
            {
                unLog.Info("INICIA ObtieneParametrosDeNodo()");
                this.StoreParametros.DataSource = 
                    DAOInfoOnBoarding.ObtieneParametrosDeNodo(IdNodo, 
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabAdminInfoOnBoarding);
                unLog.Info("TERMINA ObtieneParametrosDeNodo()");

                this.StoreParametros.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Parámetros", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al establecer los Parámetros del Nodo").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Añadir Parámetros de la pestaña de Parámetros,
        /// llamando a la inserción del nuevo parámetro
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddParametro_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminInfoOnBoarding);

            try
            {
                int idNodoSeleccionado = int.Parse(this.hdnIdNodo.Text);

                ParametroNodo unParametro = new ParametroNodo();
                unParametro.ID_ScoreNodo = idNodoSeleccionado;
                unParametro.ValorKey = this.txtKeyNuevoParametro.Text;
                unParametro.DescripcionValor = this.txtDescNuevoParametro.Text;

                log.Info("INICIA CreaNuevoParametro()");
                DataTable dt = LNInfoOnBoarding.CreaNuevoParametro(unParametro, this.Usuario, LH_ParabAdminInfoOnBoarding);
                log.Info("TERMINA CreaNuevoParametro()");

                string msj = dt.Rows[0]["Mensaje"].ToString();
                string idParametroNuevo = dt.Rows[0]["ID_NuevoParametro"].ToString();

                if (idParametroNuevo == "-1")
                {
                    X.Msg.Alert("Nuevo Parámetro", msj).Show();
                }
                else
                {
                    X.Msg.Alert("Nuevo Parámetro", "<br />" + msj + "<br /> <b> E X I T O S A M E N T E </b> <br /> ").Show();
                    FormPanelNuevoParametro.Reset();

                    LlenaGridParametros(idNodoSeleccionado);
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Parámetro", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Nuevo Parámetro", "Ocurrió un error al generar el Parámetro del Nodo").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar de la ventana de edición de parámetro,
        /// llamando a la actualización del parámetro
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarEdicionParametro_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminInfoOnBoarding);

            try
            {
                unLog.Info("INICIA ModificaParametro()");
                string resp = LNInfoOnBoarding.ModificaParametro(Convert.ToInt32(this.hdnIdParametro.Value),
                    this.txtKeyParametro.Text, this.txtDescParametro.Text, true, this.Usuario, LH_ParabAdminInfoOnBoarding);
                unLog.Info("TERMINA ModificaParametro()");

                if (!resp.Contains("OK"))
                {
                    X.Msg.Alert("Actualización de Parámetro", resp).Show();
                }
                else
                {
                    X.Msg.Notify("", "Parámetro Actualizado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                    LlenaGridParametros(Convert.ToInt32(this.hdnIdNodo.Value));
                    WdwEditarParametro.Hide();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Parámetro", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Actualización de Parámetro", "Ocurrió un error al actualizar los datos del Parámetro").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminInfoOnBoarding);
            string msjResp = "";

            try
            {
                ParametroNodo unParametro = new ParametroNodo();
                string json = String.Format("[{0}]", e.ExtraParams["Values"]);

                IDictionary<string, string>[] parametroSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (parametroSeleccionado == null || parametroSeleccionado.Length < 1)
                {
                    return;
                }

                foreach (KeyValuePair<string, string> column in parametroSeleccionado[0])
                {
                    switch (column.Key)
                    {
                        case "ID_ValorNodo": unParametro.ID_ValorNodo = int.Parse(column.Value); break;
                        case "ID_ScoreNodo": unParametro.ID_ScoreNodo = int.Parse(column.Value); break;
                        case "ValorKey": unParametro.ValorKey = column.Value; break;
                        case "DescripcionValor": unParametro.DescripcionValor = column.Value; break;
                        default:
                            break;
                    }
                }

                String comando = e.ExtraParams["Comando"];
                hdnIdParametro.Value = unParametro.ID_ValorNodo;

                switch (comando)
                {
                    case "Edit":
                        txtKeyParametro.Text = unParametro.ValorKey;
                        txtDescParametro.Text = unParametro.DescripcionValor;
                        WdwEditarParametro.Title += " - " + unParametro.ValorKey;
                        WdwEditarParametro.Show();                        

                        break;

                    case "Delete":
                        log.Info("INICIA EliminaParametro()");
                        msjResp = LNInfoOnBoarding.ModificaParametro(Convert.ToInt32(this.hdnIdParametro.Value), "", "", false, 
                            this.Usuario, LH_ParabAdminInfoOnBoarding);
                        log.Info("TERMINA EliminaParametro()");

                        if (!msjResp.Contains("OK"))
                        {
                            X.Msg.Alert("Eliminación de Parámetro", msjResp).Show();
                        }
                        else
                        {
                            X.Msg.Notify("", "Parámetro Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                            LlenaGridParametros(Convert.ToInt32(this.hdnIdNodo.Value));
                        }

                        break;

                    default: break;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Parámetros", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del panel izquierdo, limpiando los controles,
        /// páneles y grids asociados a alguna previa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarIzq_Click(object sender, EventArgs e)
        {
            FormPanelNuevoNodo.Reset();
            txtBusquedaNodo.Reset();
            StoreNodos.RemoveAll();
            txtNombreKey.Reset();
            txtNombreNodo.Reset();
            PanelCentralNodos.Disabled = true;
            this.FormPanelInfoAd.Show();
            PanelCentralNodos.Title = "";
        }

    }
}
