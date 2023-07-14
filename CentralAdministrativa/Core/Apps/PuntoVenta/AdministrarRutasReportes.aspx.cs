using DALAutorizador.BaseDatos;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using DALPuntoVentaWeb.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace TpvWeb
{
    public partial class AdministrarRutasReportes : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER PARABILIA Administrar Rutas Reportes
        private LogHeader LH_ParabAdminRutasReportes = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Administrar Rutas de Reportes
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabAdminRutasReportes.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabAdminRutasReportes.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabAdminRutasReportes.User = this.Usuario.ClaveUsuario;
            LH_ParabAdminRutasReportes.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabAdminRutasReportes);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdministrarRutasReportes Page_Load()");

                if (!IsPostBack)
                {
                    EstableceCombos();
                }

                log.Info("TERMINA AdministrarRutasReportes Page_Load()");
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
        /// Establece los valores de los controles combo con sus valores de catálogos
        /// </summary>
        protected void EstableceCombos()
        {
            LogPCI log = new LogPCI(LH_ParabAdminRutasReportes);

            log.Info("INICIA ListaTiposColectivaEmisor()");
            DataTable dtTiposColectiva = DAOColectiva.ListaTiposColectivaEmisor(LH_ParabAdminRutasReportes);
            log.Info("TERMINA ListaTiposColectivaEmisor()");

            this.StoreTipoColectiva.DataSource = dtTiposColectiva;
            this.StoreTipoColectiva.DataBind();

            log.Info("INICIA ListaTiposServicio()");
            DataSet dsTiposServicio = DAOReportes.ListaTiposServicios(LH_ParabAdminRutasReportes);
            log.Info("TERMINA ListaTiposServicio()");

            this.StoreTipoServicio.DataSource = dsTiposServicio;
            this.StoreTipoServicio.DataBind();

            log.Info("INICIA ListaClasificacion()");
            DataSet dsClasificacion = DAOReportes.ListaClasificacion(LH_ParabAdminRutasReportes);
            log.Info("TERMINA ListaClasificacion()");


            log.Info("INICIA ListaClasificacionParametros()");
            this.StoreClasif2.DataSource = DAOAdministrarColectivas.ListaClasificacionParametros(
                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAdminRutasReportes);
            log.Info("TERMINA ListaClasificacionParametros()");
            this.StoreClasif2.DataBind();

            log.Info("INICIA LlenaParametrosColectiva()");
            LlenaParametrosColectiva();
            log.Info("TERMINA LlenaParametrosColectiva()");

            this.StoreClasificacion.DataSource = dsClasificacion;
            this.StoreClasificacion.DataBind();
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del panel izquierdo, limpiando los controles,
        /// páneles y grids asociados a alguna previa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarIzq_Click(object sender, EventArgs e)
        {
            this.cBoxTipoColec.Reset();
            this.txtColectiva.Reset();
            StoreColectivas.RemoveAll();

            LimpiaSeleccionPrevia();
            PanelCentral.Disabled = true;
        }

        /// <summary>
        /// Llena el grid de resultados de colectivas con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminRutasReportes);

            try
            {
                LimpiaSeleccionPrevia();

                unLog.Info("INICIA ListaColectivasPorTipo()");
                DataSet dsColectivas = DAOColectiva.ListaColectivasPorTipo(
                    int.Parse(this.cBoxTipoColec.SelectedItem.Value), this.txtColectiva.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminRutasReportes);
                unLog.Info("TERMINA ListaColectivasPorTipo()");

                if (dsColectivas.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Colectivas", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreColectivas.DataSource = dsColectivas;
                    StoreColectivas.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Colectivas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Colectivas", "Ocurrió un error al realizar la bÚsqueda.").Show();
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
        /// Limpia los controles, páneles, grids asociados a la selección previa de
        /// una colectiva en el Grid de Resultados
        /// </summary>
        protected void LimpiaSeleccionPrevia()
        {
            this.StoreColectivas.RemoveAll();

            this.cBoxReportes.Reset();
            this.StoreValoresReportes.RemoveAll();

            this.StoreValoresParametros2.RemoveAll();

            this.FormPanelReportes.Show();
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowResultados_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdColectiva = 0;
                string NombreColectiva = "";

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] colectiva = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in colectiva[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Colectiva": IdColectiva = int.Parse(column.Value); break;
                        case "NombreORazonSocial": NombreColectiva = column.Value; break;
                        default: break;
                    }
                }

                LimpiaSeleccionPrevia();

                this.hdnIdColectiva.Text = IdColectiva.ToString();
                PanelCentral.Title = cBoxTipoColec.SelectedItem.Text + " - " + NombreColectiva;

                LlenaGridRutasReportes(IdColectiva);
                LlenaReportesSinAsignar(IdColectiva);
                EstableceCombos();
                PanelCentral.Disabled = false;

                if (cBoxTipoColec.SelectedItem.Text == "Emisor")
                {
                    FormPanelParametros.Disabled = false;
                }
                else
                {
                    FormPanelParametros.Disabled = true;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Colectivas", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ParabAdminRutasReportes);
                unLog.ErrorException(ex);
                X.Msg.Alert("Colectivas", "Ocurrió un error al obtener la información de la Colectiva.").Show();
            }
        }

        /// <summary>
        /// Llena el grid de Rutas Reportes con la información de base de datos
        /// </summary>
        protected void LlenaGridRutasReportes(int IdColectiva)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminRutasReportes);

            try
            {
                unLog.Info("INICIA ListaRutasReportes()");
                DataSet dsRutasReportes = DAOReportes.ListaRutasReportes(IdColectiva, LH_ParabAdminRutasReportes);
                unLog.Info("TERMINA ListaRutasReportes()");

                this.StoreValoresReportes.DataSource = dsRutasReportes;
                this.StoreValoresReportes.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Rutas Reportes", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Rutas Reportes", "Ocurrió un error al obtener los Rutas Reportes").Show();
            }
        }

        /// <summary>
        /// Llena el comno de Reportes sin asignar con la información de base de datos
        /// </summary>
        protected void LlenaReportesSinAsignar(int IdColectiva)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminRutasReportes);

            try
            {
                unLog.Info("INICIA ListaRutasReportes()");
                DataSet dsReportes = DAOReportes.ListaReportesSinAsignar(IdColectiva, LH_ParabAdminRutasReportes);
                unLog.Info("TERMINA ListaRutasReportes()");

                this.StoreReportes.DataSource = dsReportes;
                this.StoreReportes.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Reportes Sin Asignar", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Reportes Sin Asignar", "Ocurrió un error al obtener los Reportes Sin Asignar").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Asignar Reporte 
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddReportes_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminRutasReportes);

            try
            {
                int IdColectiva = Convert.ToInt32(this.hdnIdColectiva.Text);

                log.Info("INICIA AsignaReporteColectiva()");
                LNReportes.AsignaReporteColectiva(
                    int.Parse(cBoxReportes.SelectedItem.Value), IdColectiva, this.Usuario, LH_ParabAdminRutasReportes);
                log.Info("TERMINA AsignaReporteColectiva()");

                X.Msg.Notify("", "Reporte Asignado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                cBoxReportes.Reset();

                LimpiaSeleccionPrevia();
                LlenaGridRutasReportes(IdColectiva);
                LlenaReportesSinAsignar(IdColectiva);
                LlenaParametrosColectiva();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Asignación de Reporte", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Asignación de Reporte", "Ocurrió un error al asignar el Reporte a la Colectiva").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoReportes(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminRutasReportes);

            try
            {
                string json = String.Format("[{0}]", e.ExtraParams["Values"]);
                IDictionary<string, string>[] reporteSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                string Nombre = "";
                string Ruta = "";
                string NombreArchivo = "";
                int IdReporte = 0;
                int IdTipoServicio = 0;
                int IdClasificacion = 0;
                int IdReporteColectivaConfiguracion = 0;
                int IdReporteColectiva = 0;
                int EsActivo = 0;
                TimeSpan ts = new TimeSpan(0, 0, 0);

                if (reporteSeleccionado == null || reporteSeleccionado.Length < 1)
                {
                    return;
                }

                foreach (KeyValuePair<string, string> column in reporteSeleccionado[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Reporte": IdReporte = int.Parse(column.Value); break;
                        case "Ruta": Ruta = column.Value; break;
                        case "Nombre": Nombre = column.Value; break;
                        case "NombreArchivo": NombreArchivo = column.Value; break;
                        case "HoraEjecucion": this.tfHoraEjecucion.Value = column.Value == null ? ts : TimeSpan.Parse(column.Value.ToString()); break;
                        case "ID_TipoServicio": IdTipoServicio = column.Value == null ? 1 : int.Parse(column.Value); break;
                        case "ID_Clasificacion": IdClasificacion = column.Value == null ? 1 : int.Parse(column.Value); break;
                        case "ID_ReporteColectiva": IdReporteColectiva = column.Value == null ? 0 : int.Parse(column.Value); break;
                        case "ID_ReporteColectivaConfiguracion": IdReporteColectivaConfiguracion = column.Value == null ? 0 : int.Parse(column.Value); break;
                        case "EsActivo": EsActivo = column.Value == "True" ? 1 : 0; break;
                        default:
                            break;
                    }
                }

                String comando = e.ExtraParams["Comando"];

                switch (comando)
                {
                    case "Edit":
                        txtNombre.Text = Nombre;
                        txtRuta.Text = Ruta;
                        txtArchivo.Text = NombreArchivo;
                        cboxTipoServicio.Value = IdTipoServicio;
                        cboxClasificacion.Value = IdClasificacion;
                        hdnIdReporteColectivaConfiguracion.Text = IdReporteColectivaConfiguracion.ToString();
                        hdnIdReporteColectiva.Text = IdReporteColectiva.ToString();
                        hdnEstatus.Text = EsActivo.ToString();

                        wdwEditar.Title += " - " + Nombre;
                        wdwEditar.Show();
                        break;

                    case "CambiaEstatus":
                        EsActivo = EsActivo.Equals(1) ? 0 : 1;

                        log.Info("INICIA ModificaEstatusConfigReporteColectiva()");
                        LNReportes.ModificaEstatusConfigReporteColectiva(IdReporteColectivaConfiguracion, EsActivo,
                            this.Usuario, LH_ParabAdminRutasReportes);
                        log.Info("TERMINA ModificaEstatusConfigReporteColectiva()");

                        LlenaGridRutasReportes(int.Parse(hdnIdColectiva.Text));
                        if (EsActivo == 1)
                        {
                            X.Msg.Notify("", "Reporte Activado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                        }
                        else
                        {
                            X.Msg.Notify("", "Reporte Desactivado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                        }

                        break;

                    default: break;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Reportes", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Reportes", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar la Configuración de los Reportes
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminRutasReportes);

            try
            {
                int IdReporteColectivaConfiguracion = int.Parse(hdnIdReporteColectivaConfiguracion.Text);
                int IdReporteColectiva = int.Parse(hdnIdReporteColectiva.Text);
                int IdClasificacion = int.Parse(this.cboxClasificacion.SelectedItem.Value);
                int IdTipoServicio = int.Parse(this.cboxTipoServicio.SelectedItem.Value);
                string Ruta = this.txtRuta.Text;
                string NombreArchivo = this.txtArchivo.Text;
                TimeSpan horaEjec = TimeSpan.Parse(this.tfHoraEjecucion.SelectedTime.ToString());

                pCI.Info("INICIA ActualizaConfigReporteColectiva()");
                LNReportes.ActualizaConfigReporteColectiva(IdReporteColectivaConfiguracion, IdReporteColectiva, 
                    IdClasificacion, IdTipoServicio, Ruta, NombreArchivo, horaEjec, this.Usuario, LH_ParabAdminRutasReportes);
                pCI.Info("TERMINA ActualizaConfigReporteColectiva()");

                LlenaGridRutasReportes(int.Parse(hdnIdColectiva.Text));

                this.wdwEditar.Hide();
                X.Msg.Notify("Actualización de Configuración", "Modificación de Configuración de Reporte <br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Configuración", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Actualización de Configuración", "Ocurrió un error al actualizar la configuración del reporte.").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminRutasReportes);

            try
            {
                Parametro unParametro = new Parametro();
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
                        case "ID_Parametro": unParametro.ID_Parametro = int.Parse(column.Value); break;
                        case "Nombre": unParametro.Nombre = column.Value; break;
                        case "Descripcion": unParametro.Descripcion = column.Value; break;
                        case "Valor": unParametro.Valor = column.Value; break;
                        case "ValorPrestablecido": unParametro.Preestablecido = bool.Parse(column.Value); break;
                        case "ID_ValorPrestablecido": unParametro.ID_ParametroPrestablecido = int.Parse(column.Value); break;
                        default:
                            break;
                    }
                }

                String comando = e.ExtraParams["Comando"];
                hdnIdParametro.Value = unParametro.ID_Parametro;
                hdnParametroNombre.Value = unParametro.Nombre;

                switch (comando)
                {
                    case "Edit":
                        if (unParametro.Preestablecido)
                        {
                            StoreValoresPrestablecidos2.RemoveAll();

                            log.Info("INICIA ListaCatalogoValoresParametro()");
                            StoreValoresPrestablecidos2.DataSource =
                                DAOAdministrarColectivas.ListaCatalogoValoresParametro(
                                unParametro.ID_Parametro, this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                                LH_ParabAdminRutasReportes);
                            log.Info("TERMINA ListaCatalogoValoresParametro()");
                            StoreValoresPrestablecidos2.DataBind();

                            cBoxValorParametro.Value = unParametro.ID_ParametroPrestablecido;
                            txtParametroCombo.Text = unParametro.Descripcion;
                            WdwValorParametroCombo2.Title += " - " + unParametro.Nombre;
                            WdwValorParametroCombo2.Show();
                        }
                        else
                        {
                            FormPanelValorParamTxt.Reset();

                            txtValorParametro.Text = unParametro.Valor;
                            txtParametro.Text = unParametro.Descripcion;
                            WdwValorParametroTexto.Title += " - " + unParametro.Nombre;
                            WdwValorParametroTexto.Show();
                        }

                        break;

                    case "Delete":
                        Int64 IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);

                        log.Info("INICIA BorraParametroAColectiva()");
                        LNAdministrarColectivas.BorraParametroAColectiva(unParametro.ID_Parametro,
                            IdColectiva, this.Usuario, LH_ParabAdminRutasReportes);
                        log.Info("TERMINA BorraParametroAColectiva()");

                        X.Msg.Notify("", "Parámetro Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        this.cBoxClasificacion2.FireEvent("select");
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
        /// Controla el evento Click al botón Asignar Parámetro de la pestaña de Parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddParametros_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminRutasReportes);

            try
            {
                long IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);

                log.Info("INICIA AgregaParametroAContrato()");
                string resp = LNAdministrarColectivas.AgregaParametroAContrato(
                    int.Parse(cBoxParametros.SelectedItem.Value), IdColectiva, this.Usuario,
                    LH_ParabAdminRutasReportes);
                log.Info("TERMINA AgregaParametroAContrato()");

                cBoxParametros.ClearValue();

                if (resp.ToUpper().Contains("ERROR"))
                {
                    X.Msg.Notify("Asignación de Parámetro", "<b>" + resp + "</b> <br /> <br /> ").Show();
                }
                else
                {
                    X.Msg.Notify("", "Parámetro Asignado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                    this.cBoxClasificacion2.FireEvent("select");
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Asignación de Parámetro", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Asignación de Parámetro", "Ocurrió un error al asignar el Parámetro a la Colectiva").Show();
            }
        }

        /// <summary>
        /// Controla el evento Select a los ítems del combo de clasificación
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void SeleccionaClasificacion(object sender, EventArgs e)
        {
            LlenaParametrosColectiva();
        }

        /// <summary>
        /// Establece los valores de los controles de parámetros de la colectiva,
        /// llamando a los objetos de datos que obtienen los valores
        /// </summary>
        protected void LlenaParametrosColectiva()
        {
            LogPCI log = new LogPCI(LH_ParabAdminRutasReportes);

            try
            {
                Int64 IdColectiva = Convert.ToInt64(hdnIdColectiva.Text);

                log.Info("INICIA ListaParametrosSinAsignar()");
                this.StoreParametros.DataSource = DAOReportes.ListaParametrosSinAsignarSFTPEmisor(
                    //8 //int.Parse(cBoxClasificacion2.SelectedItem.Value) 
                    IdColectiva, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminRutasReportes);
                log.Info("TERMINA ListaParametrosSinAsignar()");
                this.StoreParametros.DataBind();

                log.Info("INICIA ListaParametrosAsignados()");
                this.StoreValoresParametros2.DataSource = DAOReportes.ListaParametrosAsignadosSFTPEmisor(
                    //8, //int.Parse(cBoxClasificacion2.SelectedItem.Value)
                    IdColectiva, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminRutasReportes);
                log.Info("TERMINA ListaParametrosAsignados()");
                this.StoreValoresParametros2.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Parámetros", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al obtener los Paámetros de la Colectiva").Show();
            }
        }







        /// <summary>
        /// Controla el evento Click al botón Guardar Cambio de las ventanas
        /// de edición de valor del parámetro de contrato
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarValorParametro_Click2(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminRutasReportes);

            try
            {
                //ActualizaValorParametro();
                Int64 IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);
                Parametro elParametro = new Parametro();
                String Origen = hdnOrigen.Value.ToString();

                elParametro.ID_Colectiva = IdColectiva;
                elParametro.ID_Parametro = int.Parse(this.hdnIdParametro.Value.ToString());
                elParametro.Valor = Origen == "CMB" ? this.cBoxValorParametro.SelectedItem.Value : this.txtValorParametro.Text;
                elParametro.ID_ParametroPrestablecido = Origen == "CMB" ?
                    int.Parse(this.cBoxValorParametro.SelectedItem.Value) : -1;

                decimal MaxMarkUp = Convert.ToDecimal(Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "MaxMarkUp").Valor);

                if (hdnParametroNombre.Value.ToString() == "@MarkUpPorcentaje")
                {
                    try
                    {
                        var valor = decimal.Parse(elParametro.Valor);
                    }
                    catch (Exception)
                    {
                        X.Msg.Alert("Actualización de Parámetros", "El valor de este parámetro debe ser expresado en decimal. Ej.: 0.04").Show();
                        return;
                    }

                    if (Decimal.Parse(elParametro.Valor) > MaxMarkUp)
                    {
                        X.Msg.Alert("Actualización de Parámetros", "El valor de este parámetro no puede ser mayor a: " + MaxMarkUp).Show();
                        return;
                    }
                }

                pCI.Info("INICIA ModificaValorParametro()");
                LNAdministrarColectivas.ModificaValorParametro(elParametro, this.Usuario, LH_ParabAdminRutasReportes);
                pCI.Info("TERMINA ModificaValorParametro()");

                this.WdwValorParametroTexto.Hide();
                this.WdwValorParametroCombo2.Hide();

                this.cBoxClasificacion2.FireEvent("select");

                X.Msg.Notify("Actualización de Parámetros", "Modificación de Parámetro <br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Parámetros", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Actualización de Parámetros", "Ocurrió un error al actualizar el valor del Parámetro").Show();
            }
        }
    }
}