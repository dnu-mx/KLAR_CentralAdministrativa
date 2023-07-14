using DALCentralAplicaciones;
using DALValidacionesBatchPPF.BaseDatos;
using DALValidacionesBatchPPF.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.Xml;
using System.Xml.Xsl;


namespace ValidacionesBatch
{
    public partial class ConsultaIncidencias : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Consulta de Incidencias
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    PreLoadMainScreenControls();

                    PreLoadFrameScreenControls();

                    PreCargaHistorialIncidencias();
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Realiza el pre llenado de los controles de la pantalla principal
        /// </summary>
        protected void PreLoadMainScreenControls()
        {
            //Prestablecemos las fechas de consulta
            dfFechaInicial.MaxDate = DateTime.Today;
            dfFechaInicial.MinDate = DateTime.Today.AddDays(-180);
            dfFechaInicial.SetValue(DateTime.Today.AddDays(-2));

            dfFechaFinal.MaxDate = DateTime.Today;
            dfFechaFinal.SetValue(DateTime.Today);
        }

        /// <summary>
        /// Realiza el pre llenado de los controles de la pantalla secundaria
        /// </summary>
        protected void PreLoadFrameScreenControls()
        {
            //Prestablecemos el periodo de consulta de las operaciones
            dfFechaInicialOper.MaxDate = DateTime.Today;
            dfFechaInicialOper.MinDate = DateTime.Today.AddDays(-180);
            dfFechaInicialOper.SetValue(DateTime.Today.AddDays(-180));

            dfFechaFinalOper.MaxDate = DateTime.Today;
            dfFechaFinalOper.SetValue(DateTime.Today);

            //Prestablecemos el periodo de consulta de las incidencias
            dfFechaInicialInc.MaxDate = DateTime.Today;
            dfFechaInicialInc.MinDate = DateTime.Today.AddDays(-180);
            dfFechaInicialInc.SetValue(DateTime.Today.AddDays(-180));

            dfFechaFinalInc.MaxDate = DateTime.Today;
            dfFechaFinalInc.SetValue(DateTime.Today);

            //Se consultael catálogo de Reglas y se establece en ambos controles
            this.StoreReglas.DataSource = this.StoreReglasInc.DataSource = 
                DAOEfectivaleOffline.ListaReglas(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
            this.StoreReglas.DataBind();
            this.StoreReglasInc.DataBind();
        }

        /// <summary>
        /// Realiza y carga en el Grid de Incidencias una pre consulta del historial 
        /// de ellas en los últimos 2 días
        /// </summary>
        protected void PreCargaHistorialIncidencias()
        {
            try
            {
                StoreIncidencias.RemoveAll();

                StoreIncidencias.DataSource = DAOEfectivaleOffline.ObtieneIncidencias(
                    Convert.ToDateTime(this.dfFechaInicial.SelectedDate), Convert.ToDateTime(this.dfFechaFinal.SelectedDate),
                    String.IsNullOrEmpty(this.cmbRegla.SelectedItem.Value) ? -1 : Convert.ToInt32(this.cmbRegla.SelectedItem.Value),
                    String.IsNullOrEmpty(this.cmbAccion.SelectedItem.Value) ? -1 : Convert.ToInt32(this.cmbAccion.SelectedItem.Value),
                    this.txtTarjeta.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreIncidencias.DataBind();

                this.GridIncidencias.Reload();
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Incidencias", "Ocurrió un Error en la Pre Consulta de Incidencias").Show();

            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Incidencias", ex.Message).Show();
            }

        }

        /// <summary>
        /// Controla el evento Click al botón Buscar, invocando a la 
        /// búsqueda de incidencias en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(this.txtTarjeta.Text))
                {
                    Int64 tarjeta;

                    if (!Int64.TryParse(this.txtTarjeta.Text, out tarjeta))
                    {
                        X.Msg.Alert("Filtros de búsqueda", "La Tarjeta debe ser numérica").Show();
                        return;
                    }
                }

                StoreIncidencias.RemoveAll();

                StoreIncidencias.DataSource = DAOEfectivaleOffline.ObtieneIncidencias(
                    Convert.ToDateTime(this.dfFechaInicial.SelectedDate), Convert.ToDateTime(this.dfFechaFinal.SelectedDate),
                    String.IsNullOrEmpty(this.cmbRegla.SelectedItem.Value) ? -1 : Convert.ToInt32(this.cmbRegla.SelectedItem.Value),
                    String.IsNullOrEmpty(this.cmbAccion.SelectedItem.Value) ? -1 : Convert.ToInt32(this.cmbAccion.SelectedItem.Value),
                    this.txtTarjeta.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreIncidencias.DataBind();
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Incidencias", "Ocurrió un Error en la Consulta de Incidencias").Show();
                
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Incidencias", ex.Message).Show();
            }
           
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar, limpiando todos los controles de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            FormPanelBusqueda.Reset();

            dfFechaInicial.SetValue(DateTime.Today.AddDays(-2));

            dfFechaFinal.SetValue(DateTime.Today);

            StoreIncidencias.RemoveAll();
        }

        /// <summary>
        /// Controla el evento de selección de una celda del grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void Cell_Click(object sender, DirectEventArgs e)
        {
            CellSelectionModel sm = this.GridIncidencias.SelectionModel.Primary as CellSelectionModel;

            if (String.Compare(sm.SelectedCell.Name, "Tarjeta") != 0  &&
                String.Compare(sm.SelectedCell.Name, "Afiliacion") != 0)
            {
                return;
            }

            switch (sm.SelectedCell.Name)
            {
                case "Tarjeta":
                    if (!String.IsNullOrEmpty(sm.SelectedCell.Value))
                    {
                        WdwDetalleTarjeta.Title += sm.SelectedCell.Value;
                        this.hdnTarjeta.Text = sm.SelectedCell.Value.Substring(1,16);

                        LimpiaTabOperaciones();
                        PreCargaOperaciones();

                        LimpiaTabIncidencias();
                        PreCargaIncidenciasPorTarjeta();

                        this.WdwDetalleTarjeta.Show();
                        FormPanelOperaciones.Show();
                    }
                    break;

                case "Afiliacion":
                    if (!String.IsNullOrEmpty(sm.SelectedCell.Value))
                    {
                        VentanaAfiliacion(sm.SelectedCell.Value);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Controla el evento SUBMIT al querer exportar al formato seleccionado
        /// los resultados de la consulta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void StoreSubmit(object sender, StoreSubmitDataEventArgs e)
        {
            string format = this.FormatType.Value.ToString();

            XmlNode xml = e.Xml;

            this.Response.Clear();

            switch (format)
            {
                case "xls":
                    this.Response.ContentType = "application/vnd.ms-excel";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.xls");
                    XslCompiledTransform xtExcel = new XslCompiledTransform();
                    xtExcel.Load(Server.MapPath("xslFiles/Excel.xsl"));
                    xtExcel.Transform(xml, null, Response.OutputStream);

                    break;

                case "csv":
                    this.Response.ContentType = "application/octet-stream";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.csv");
                    XslCompiledTransform xtCsv = new XslCompiledTransform();
                    xtCsv.Load(Server.MapPath("xslFiles/Csv.xsl"));
                    xtCsv.Transform(xml, null, Response.OutputStream);

                    break;
            }
            this.Response.End();
        }

        /// <summary>
        /// Solicita la consulta de datos de la afiliación con el valor indicado y muestra
        /// la venta correspondiente
        /// </summary>
        /// <param name="afiliacion">Afiliación por consultar y mostrar sus datos</param>
        protected void VentanaAfiliacion(String afiliacion)
        {
            try
            {
                DataSet dsAfiliacion =
                    DAOEfectivaleOffline.ObtieneDatosAfiliacion(afiliacion,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                this.txtAfiliacion.Text = dsAfiliacion.Tables[0].Rows[0]["Afiliacion"].ToString().Trim();
                this.txtNombre.Text = dsAfiliacion.Tables[0].Rows[0]["Nombre"].ToString().Trim();
                this.txtPropietario.Text = dsAfiliacion.Tables[0].Rows[0]["Propietario"].ToString().Trim();
                this.txtRazonSocial.Text = dsAfiliacion.Tables[0].Rows[0]["Razon_Social"].ToString().Trim();
                this.txtRFC.Text = dsAfiliacion.Tables[0].Rows[0]["RFC"].ToString().Trim();

                this.txtDomicilio.Text = dsAfiliacion.Tables[0].Rows[0]["Domicilio"].ToString().Trim();
                this.txtColonia.Text = dsAfiliacion.Tables[0].Rows[0]["Colonia"].ToString().Trim();
                this.txtCodigoPostal.Text = dsAfiliacion.Tables[0].Rows[0]["CP"].ToString().Trim();
                this.txtEstado.Text = dsAfiliacion.Tables[0].Rows[0]["Estado"].ToString().Trim();
                this.txtDescripcion.Text = dsAfiliacion.Tables[0].Rows[0]["Descripcion_SIC"].ToString().Trim();

                this.WdwAfiliacion.Show();
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Afiliación", "Ocurrió un Error en la Consulta de Datos de la Afiliación").Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Afiliación", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Aceptar de la ventana Afiliación, para ocultarla
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            this.WdwAfiliacion.Hide();
        }

        /// <summary>
        /// Realiza y carga en el Grid de Operaciones una pre consulta de las operaciones 
        /// de la tarjeta seleccionada en los últimos 6 meses
        /// </summary>
        protected void PreCargaOperaciones()
        {
            try
            {
                StoreResultadosOper.DataSource = DAOEfectivaleOffline.ObtieneOperacionesPorTarjeta(
                    Convert.ToDateTime(this.dfFechaInicialOper.SelectedDate),
                    Convert.ToDateTime(this.dfFechaFinalOper.SelectedDate),
                    this.txtAfiliacionOper.Text, this.hdnTarjeta.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreResultadosOper.DataBind();
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", "Ocurrió un Error en la Pre Consulta de Operaciones").Show();

            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar de la pestaña de Operaciones, invocando a la 
        /// búsqueda de operaciones de la tarjeta en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarOper_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(this.txtAfiliacionOper.Text))
                {
                    Int64 afil;

                    if (!Int64.TryParse(this.txtAfiliacionOper.Text, out afil))
                    {
                        X.Msg.Alert("", "La Afiliación debe ser numérica").Show();
                        return;
                    }
                }

                StoreResultadosOper.RemoveAll();

                StoreResultadosOper.DataSource = DAOEfectivaleOffline.ObtieneOperacionesPorTarjeta(
                    Convert.ToDateTime(this.dfFechaInicialOper.SelectedDate),
                    Convert.ToDateTime(this.dfFechaFinalOper.SelectedDate),
                    this.txtAfiliacionOper.Text, this.hdnTarjeta.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreResultadosOper.DataBind();
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", "Ocurrió un Error en la Consulta de Operaciones").Show();

            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar de la pestaña de Operaciones,
        /// limpiando todos los controles de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarOper_Click(object sender, EventArgs e)
        {
            LimpiaTabOperaciones();
        }

        /// <summary>
        /// Restablece los controles de la pestaña de Operaciones al valor de carga origen
        /// </summary>
        protected void LimpiaTabOperaciones()
        {
            FormPanelBuscarOperaciones.Reset();
            this.txtAfiliacionOper.Reset();

            dfFechaInicialOper.SetValue(DateTime.Today.AddDays(-180));

            dfFechaFinalOper.SetValue(DateTime.Today);

            StoreResultadosOper.RemoveAll();
        }

        /// <summary>
        /// Controla el evento de selección de una celda del grid de Operaciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void CellGridResultadosOper_Click(object sender, DirectEventArgs e)
        {
            CellSelectionModel sm = GridResultadosOper.SelectionModel.Primary as CellSelectionModel;

            if (String.Compare(sm.SelectedCell.Name, "Afiliacion") != 0)
            {
                return;
            }

            if (!String.IsNullOrEmpty(sm.SelectedCell.Value))
            {
                VentanaAfiliacion(sm.SelectedCell.Value);
            }
        }

        /// <summary>
        /// Realiza y carga en el Grid Resultados de Incidencias una pre consulta
        /// de ellas de la tarjeta seleccionada en los últimos 6 meses
        /// </summary>
        protected void PreCargaIncidenciasPorTarjeta()
        {
            try
            {
                StoreResultadosIncidencias.DataSource = DAOEfectivaleOffline.ObtieneIncidenciasPorTarjeta(
                    Convert.ToDateTime(this.dfFechaInicialInc.SelectedDate), Convert.ToDateTime(this.dfFechaFinalInc.SelectedDate),
                    String.IsNullOrEmpty(this.cmbReglasInc.SelectedItem.Value) ? -1 : Convert.ToInt32(this.cmbReglasInc.SelectedItem.Value),
                    String.IsNullOrEmpty(this.cmbAccionInc.SelectedItem.Value) ? -1 : Convert.ToInt32(this.cmbAccionInc.SelectedItem.Value),
                    this.hdnTarjeta.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreResultadosIncidencias.DataBind();
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Incidencias", "Ocurrió un Error en la Pre Consulta de Incidencias").Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Incidencias", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar de la pestaña de Incidencias, invocando a la 
        /// búsqueda de operaciones de la tarjeta en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscaInc_Click(object sender, EventArgs e)
        {
            try
            {
                StoreResultadosIncidencias.RemoveAll();

                StoreResultadosIncidencias.DataSource = DAOEfectivaleOffline.ObtieneIncidenciasPorTarjeta(
                    Convert.ToDateTime(this.dfFechaInicialInc.SelectedDate), Convert.ToDateTime(this.dfFechaFinalInc.SelectedDate),
                    String.IsNullOrEmpty(this.cmbReglasInc.SelectedItem.Value) ? -1 : Convert.ToInt32(this.cmbReglasInc.SelectedItem.Value),
                    String.IsNullOrEmpty(this.cmbAccionInc.SelectedItem.Value) ? -1 : Convert.ToInt32(this.cmbAccionInc.SelectedItem.Value),
                    this.hdnTarjeta.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreResultadosIncidencias.DataBind();
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Incidencias", "Ocurrió un Error en la Consulta de Incidencias").Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Incidencias", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar de la pestaña de Incidencias,
        /// limpiando todos los controles de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiaInc_Click(object sender, EventArgs e)
        {
            LimpiaTabIncidencias();
        }

        /// <summary>
        /// Restablece los controles de la pestaña de Incidenias al valor de carga origen
        /// </summary>
        protected void LimpiaTabIncidencias()
        {
            FormPanelBuscaIncidencias.Reset();
            this.cmbReglasInc.Reset();
            this.cmbAccionInc.Reset();

            dfFechaInicialInc.SetValue(DateTime.Today.AddDays(-180));

            dfFechaFinalInc.SetValue(DateTime.Today);

            StoreResultadosIncidencias.RemoveAll();
        }

        /// <summary>
        /// Controla el evento de selección de una celda del grid de Incidencias
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void CellGridResultadosIncidencias_Click(object sender, DirectEventArgs e)
        {
            CellSelectionModel sm = GridResultadosIncidencias.SelectionModel.Primary as CellSelectionModel;

            if (String.Compare(sm.SelectedCell.Name, "Afiliacion") != 0)
            {
                return;
            }

            if (!String.IsNullOrEmpty(sm.SelectedCell.Value))
            {
                VentanaAfiliacion(sm.SelectedCell.Value);
            }
        }

    }
}