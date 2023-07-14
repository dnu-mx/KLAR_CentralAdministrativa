using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALValidacionesBatchPPF.BaseDatos;
using DALValidacionesBatchPPF.Entidades;
using DALValidacionesBatchPPF.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Xsl;


namespace ValidacionesBatch
{
    public partial class ConsultaIncidenciasVG : PaginaBaseCAPP
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
                    PagingIncidencias.PageSize =
                       Convert.ToInt32(Configuracion.Get(Guid.Parse(
                       ConfigurationManager.AppSettings["IDApplication"].ToString()),
                       "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtIncidencias", null);

                    PreLoadMainScreenControls();

                    PreLoadFrameScreenControls();

                    EstableceRolesAdmin();
                }
            }
            catch (Exception err)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(err, "");
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
            dfFechaInicial.SetValue(DateTime.Today);

            dfFechaFinal.MaxDate = DateTime.Today;
            dfFechaFinal.SetValue(DateTime.Today);

            //Se consulta el catálogo de Acciones
            this.StoreAcciones.DataSource = DAOEfectivaleOnline.ListaAcciones(
                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
            this.StoreAcciones.DataBind();

            //Se consulta el catálogo de Reglas
            this.StoreReglas.DataSource = DAOEfectivaleOnline.ListaReglas(
                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
            this.StoreReglas.DataBind();
        }

        /// <summary>
        /// Realiza el pre llenado de los controles de la pantalla secundaria
        /// </summary>
        protected void PreLoadFrameScreenControls()
        {
            //Prestablecemos el periodo de consulta de las operaciones
            dfFechaInicialOper.MaxDate = DateTime.Today;
            dfFechaInicialOper.MinDate = DateTime.Today.AddDays(-180);
            dfFechaInicialOper.SetValue(DateTime.Today);

            dfFechaFinalOper.MaxDate = DateTime.Today;
            dfFechaFinalOper.SetValue(DateTime.Today);

            //Prestablecemos el periodo de consulta de las incidencias
            dfFechaInicialInc.MaxDate = DateTime.Today;
            dfFechaInicialInc.MinDate = DateTime.Today.AddDays(-180);
            dfFechaInicialInc.SetValue(DateTime.Today);

            dfFechaFinalInc.MaxDate = DateTime.Today;
            dfFechaFinalInc.SetValue(DateTime.Today);

            PreCargaValoresCaso();
        }

        /// <summary>
        /// Precarga los catálogos relacionados con un caso
        /// </summary>
        protected void PreCargaValoresCaso()
        {
            //Prestablecemos el catálogo de dictámenes por caso
            StoreDictamen.DataSource = DAOEfectivaleOnline.ListaDictamenes(this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
            StoreDictamen.DataBind();

            //Prestablecemos el catálogo de tipos de fraude
            StoreTipoFraude.DataSource = DAOEfectivaleOnline.ListaTiposFraude(this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
            StoreTipoFraude.DataBind();
        }

        /// <summary>
        /// Establece en variables de sesión los roles Administradores que pueden cancelar,
        /// bloquear y/o desbloquear una tarjeta
        /// </summary>
        protected void EstableceRolesAdmin()
        {
            this.Usuario.Roles.Sort();
            HttpContext.Current.Session.Add("EsAdmin", false);
            HttpContext.Current.Session.Add("EsAnalista", false);

            string KeyWordSuperv = Configuracion.Get(Guid.Parse(
                ConfigurationManager.AppSettings["IDApplication"].ToString()),
                "ClaveSupervisor").Valor;
            string KeyWordAdmin = Configuracion.Get(Guid.Parse(
                ConfigurationManager.AppSettings["IDApplication"].ToString()),
                "ClaveAdministrador").Valor;
            string KeyWordAnalist = Configuracion.Get(Guid.Parse(
                ConfigurationManager.AppSettings["IDApplication"].ToString()),
                "ClaveAnalista").Valor;

            foreach (string rol in this.Usuario.Roles)
            {
                if (rol.ToUpper().Contains(KeyWordSuperv) || rol.ToUpper().Contains(KeyWordAdmin))
                {
                    HttpContext.Current.Session.Add("EsAdmin", true);
                }
                else if (rol.ToUpper().Contains(KeyWordAnalist))
                {
                    HttpContext.Current.Session.Add("EsAnalista", true);
                }
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

            dfFechaInicial.SetValue(DateTime.Today);
            dfFechaFinal.SetValue(DateTime.Today);
            txtTarjeta.Clear();
            cBoxIncidencias.Clear();
            cmbRegla.Clear();
            cmbAccion.Clear();

            btnExportExcel.Disabled = true;
            btnExportCSV.Disabled = true;
            //nmbTop.Disabled = true;
            StoreIncidencias.RemoveAll();
        }

        /// <summary>
        /// Exporta el grid de resultados al backend, para generar y exportarlo a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            string gridResultJson = e.ExtraParams["GridResult"];
            string reporte = e.ExtraParams["Reporte"];
            string reportName = reporte == "O" ? "Operaciones" : "IncidenciasPorTarjeta";

            XmlNode gridResultXml = JSON.DeserializeXmlNode("{records:{record:" + gridResultJson + "}}");
            XmlTextReader xtr = new XmlTextReader(gridResultXml.OuterXml, XmlNodeType.Element, null);

            DataSet ds = new DataSet();
            ds.ReadXml(xtr);

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(reportName);

            //Se inserta la tabla completa a la hoja de Excel
            ws.Cell(1, 1).InsertTable(ds.Tables[0].AsEnumerable());

            //Se da el formato deseado a las columnas
            ws = FormatWsColumns(ws, ws.Column(1).CellsUsed().Count(), reporte);

            //Se prepara la respuesta
            this.Response.Clear();
            this.Response.ClearContent();
            this.Response.ClearHeaders();
            this.Response.Buffer = false;

            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte_" + reportName + ".xlsx");

            //Se envía el reporte como respuesta
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(this.Response.OutputStream);
                memoryStream.Close();
            }

            this.Response.End();
        }

        /// <summary>
        /// Establece el formato deseado a las columnas de la hoja de trabajo por exportar
        /// </summary>
        /// <param name="ws">Hoja de trabajo</param>
        /// <param name="rowsNum">Total de filas de la hoja de trabajo</param>
        /// <returns></returns>
        protected IXLWorksheet FormatWsColumns(IXLWorksheet ws, int rowsNum, string idreporte)
        {
            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                switch (idreporte)
                {
                    case "T":
                        ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                        ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 18).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 19).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 20).SetDataType(XLCellValues.Number);
                        break;

                    case "O":
                        ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                        ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                        break;

                    case "I":
                        ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                        ws.Cell(rowsCounter, 9).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 10).SetDataType(XLCellValues.Number);
                        break;
                }
            }

            return ws;
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
        /// Controla el evento de selección de una celda del grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void Cell_Click(object sender, DirectEventArgs e)
        {
            CellSelectionModel sm = this.GridResultados.SelectionModel.Primary as CellSelectionModel;

            if (String.Compare(sm.SelectedCell.Name, "Tarjeta") != 0  &&
                String.Compare(sm.SelectedCell.Name, "Afiliacion") != 0)
            {
                return;
            }

            try
            {
                switch (sm.SelectedCell.Name)
                {
                    case "Tarjeta":
                        if (!String.IsNullOrEmpty(sm.SelectedCell.Value))
                        {
                            this.hdnTarjeta.Text = sm.SelectedCell.Value;

                            InitTabIncidencias();
                            InitTabOperaciones();
                            InitHeaderWdwDetalleTarjeta();

                            this.WdwDetalleTarjeta.Show();
                            FormPanelIncidencias.Show();
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
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Inicializa la pestaña de Incidencias según los valores seleccionados
        /// de la pantalla de consulta de tarjetas.
        /// </summary>
        protected void InitTabIncidencias()
        {
            LimpiaTabIncidencias();

            dfFechaInicialInc.SetValue(dfFechaInicial.Value);
            dfFechaFinalInc.SetValue(dfFechaFinal.Value);

            if (!String.IsNullOrEmpty(cmbAccion.SelectedItem.Value))
            {
                cBoxAccionesInc.SetValue(cmbAccion.Value);
            }

            if (!String.IsNullOrEmpty(cmbRegla.SelectedItem.Value))
            {
                cBoxReglasInc.SetValue(cmbRegla.Value);
            }

            PreCargaIncidenciasPorTarjeta();
        }

        /// <summary>
        /// Realiza y carga en el Grid Resultados de Incidencias una pre consulta
        /// de ellas de la tarjeta seleccionada en el periodo seleccionado
        /// </summary>
        protected void PreCargaIncidenciasPorTarjeta()
        {
            try
            {
                DataSet dsIncidencias = DAOEfectivaleOnline.ObtieneIncidenciasPorTarjeta(
                    this.hdnTarjeta.Text, Convert.ToDateTime(this.dfFechaInicialInc.SelectedDate),
                    Convert.ToDateTime(this.dfFechaFinalInc.SelectedDate),
                    String.IsNullOrEmpty(this.cBoxAccionesInc.SelectedItem.Value) ? -1 :
                        Convert.ToInt32(this.cBoxAccionesInc.SelectedItem.Value), "",
                    String.IsNullOrEmpty(this.cBoxReglasInc.SelectedItem.Value) ? -1 :
                        Convert.ToInt32(this.cBoxReglasInc.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsIncidencias.Tables[0].Rows.Count > 0)
                {
                    this.btnExportExcelInc.Disabled = false;
                    this.btnExportCSVInc.Disabled = false;

                    DateTime fechaBase = Convert.ToDateTime(dsIncidencias.Tables[0].Rows[0]["FechaBaseCaso"].ToString().Trim());
                    string fechaBaseCaso = String.Format("{0:dd/MM/yyyy}", fechaBase);

                    WdwDetalleTarjeta.Title += this.hdnTarjeta.Text + 
                    "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "Fecha de Primera Incidencia: " + fechaBaseCaso;

                    StoreResultadosIncidencias.DataSource = dsIncidencias;
                    StoreResultadosIncidencias.DataBind();
                }
            }

            catch (CAppException caEx)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Incidencias", "Ocurrió un Error en la Pre Consulta de Incidencias").Show();
            }

            catch (Exception ex)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Incidencias", ex.Message).Show();
            }
        }

        /// <summary>
        /// Inicializa la pestaña de Operaciones según los valores seleccionados
        /// de la pantalla de consulta de tarjetas.
        /// </summary>
        protected void InitTabOperaciones()
        {
            LimpiaTabOperaciones();

            dfFechaInicialOper.SetValue(dfFechaInicial.Value);
            dfFechaFinalOper.SetValue(dfFechaFinal.Value);

            PreCargaOperaciones();
        }

        /// <summary>
        /// Realiza y carga en el Grid de Operaciones una pre consulta de las operaciones 
        /// de la tarjeta seleccionada en los últimos 6 meses
        /// </summary>
        protected void PreCargaOperaciones()
        {
            try
            {
                DataSet dsOperaciones = DAOEfectivaleOnline.ObtieneOperacionesPorTarjeta(
                    this.hdnTarjeta.Text, Convert.ToDateTime(this.dfFechaInicialOper.SelectedDate),
                    Convert.ToDateTime(this.dfFechaFinalOper.SelectedDate),
                    String.IsNullOrEmpty(this.cBoxEstatusOper.SelectedItem.Value) ? -1 :
                    int.Parse(this.cBoxEstatusOper.SelectedItem.Value),
                    this.txtAfiliacionOper.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsOperaciones.Tables[0].Rows.Count > 0)
                {
                    this.btnExportExcelOper.Disabled = false;
                    this.btnExportCSVOper.Disabled = false;

                    StoreResultadosOper.DataSource = dsOperaciones;
                    StoreResultadosOper.DataBind();
                }
            }

            catch (CAppException caEx)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", "Ocurrió un Error en la Pre Consulta de Operaciones").Show();
            }

            catch (Exception ex)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", ex.Message).Show();
            }
        }

        /// <summary>
        /// Inicializa los controles del header en la pantalla de detalle de tarjeta,
        /// según su estatus en base de datos y el rol del usuario en sesión
        /// </summary>
        protected void InitHeaderWdwDetalleTarjeta()
        {
            try
            {
                string EstatusTarjeta = "";
                cBoxEstatusTarjeta.Reset();

                Boolean esAdministrador = Boolean.Parse(HttpContext.Current.Session["EsAdmin"].ToString());
                Boolean esAnalista = Boolean.Parse(HttpContext.Current.Session["EsAnalista"].ToString());

                WdwDetalleTarjeta.Title = !WdwDetalleTarjeta.Title.Contains("Fecha") ? 
                    WdwDetalleTarjeta.Title += this.hdnTarjeta.Text : WdwDetalleTarjeta.Title;

                DataSet dsEstatus = DAOEfectivaleOnline.ObtieneEstatusTarjeta(
                       this.hdnTarjeta.Text, this.Usuario,
                       Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                foreach (DataRow row in dsEstatus.Tables[0].Rows)
                {
                    if (Convert.ToBoolean(row["EstatusActual"]))
                    {
                        EstatusTarjeta = row["Descripcion"].ToString();
                        lblEstatusTarjeta.Text += EstatusTarjeta;
                        row.Delete();
                    }
                }

                dsEstatus.Tables[0].AcceptChanges();

                cBoxEstatusTarjeta.Hidden = EstatusTarjeta.ToUpper().Contains("CANCEL") ? true :
                   esAdministrador || esAnalista ? false : true;
                btnAceptarEstatus.Hidden = cBoxEstatusTarjeta.Hidden;

                if (esAnalista && (cBoxEstatusTarjeta.Hidden == false))
                {
                    foreach (DataRow row in dsEstatus.Tables[0].Rows)
                    {
                        if (row["Clave"].ToString() == Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "ClaveMA_Cancelada").Valor)
                        {
                            row.Delete();
                            break;
                        }
                    }

                    dsEstatus.Tables[0].AcceptChanges();
                }

                StoreEstatusTarjeta.DataSource = dsEstatus;
                StoreEstatusTarjeta.DataBind();
            }

            catch (CAppException)
            {
                X.Msg.Alert("Detalle Tarjeta", "Ocurrió un Error en la Consulta de Detalles de la Tarjeta").Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Detalle Tarjeta", ex.Message).Show();
            }
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

                if (dsAfiliacion.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Afiliación", "La información detallada de Afiliación no fue encontrada").Show();
                    return;
                }

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
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Afiliación", "Ocurrió un Error en la Consulta de Datos de la Afiliación").Show();
            }

            catch (Exception ex)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
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
        /// Realiza las validaciones para bloquear o desbloquear la tarjeta
        /// </summary>
        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void CambiaEstatusTarjeta(String TituloEstatus)
        {
            try
            {
                string MsjEstatus = TituloEstatus == "Bloqueo" ? " E X I T O S O " :
                    " E X I T O S A ";

                string msj = LNEfectivale.ModificaEstatusTarjeta(int.Parse(cBoxEstatusTarjeta.SelectedItem.Value),
                    this.hdnTarjeta.Text, this.Usuario);

                if (!String.IsNullOrEmpty(msj))
                {
                    X.Msg.Alert("Cambiar Estatus", msj).Show();
                }

                else
                {
                    InitHeaderWdwDetalleTarjeta();
                    X.Msg.Notify("Cambiar Estatus", TituloEstatus + " de Tarjeta <br /><br />  <b>" + MsjEstatus + "</b> <br />  <br /> ").Show();
                }
            }

            catch (CAppException caEx)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cambiar Estatus", "Ocurrió un Error durante el proceso de " + TituloEstatus + " de la Tarjeta").Show();
            }

            catch (Exception ex)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cambiar Estatus", ex.Message).Show();
            }
        }

        /// <summary>
        /// Restablece los controles de la pestaña de Operaciones al valor de carga origen
        /// </summary>
        protected void LimpiaVentanaOperacionesFraude()
        {
            FormPanelOpFraude.Reset();

            cBoxDictamen.Clear();
            cBoxTipoFraude.Clear();

            StoreOpFraude.RemoveAll();

            GridPanelOpFraude.Disabled = true;
        }

        /// <summary>
        /// Llena el Grid de Operaciones Fraudulentas
        /// </summary>
        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void LlenaGridPanelOpFraude()
        {
            try
            {
                LimpiaVentanaOperacionesFraude();

                StoreOpFraude.DataSource = DAOEfectivaleOnline.ObtieneTotalIncidenciasPorTarjeta(this.hdnTarjeta.Text,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreOpFraude.DataBind();

                WdwOperacionesFraude.Show();
            }

            catch (CAppException caEx)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cierre de Caso", "Ocurrió un Error en la Consulta de Incidencias de la Tarjeta").Show();
            }

            catch (Exception ex)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cierre de Caso", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento de Click al botón de Aceptar de cierre de caso,
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnAceptarCierre_Click(object sender, DirectEventArgs e)
        {
            try
            {
                RowSelectionModel rsm = this.GridPanelOpFraude.SelectionModel.Primary as RowSelectionModel;

                if (this.cBoxDictamen.SelectedItem.Value == "FRAU")
                {
                    if (rsm.SelectedRows.Count < 1)
                    {
                        X.Msg.Alert("Cierre de Caso", "Debe marcar al menos una operación como fraudulenta").Show();
                        return;
                    }
                }

                LNEfectivale.CierraCaso(cBoxDictamen.SelectedItem.Value,
                    String.IsNullOrEmpty(cBoxTipoFraude.SelectedItem.Value) ? -1 :
                    int.Parse(cBoxTipoFraude.SelectedItem.Value),
                    this.hdnTarjeta.Text, this.Usuario);

                if (this.cBoxDictamen.SelectedItem.Value == "FRAU")
                {
                    int fila = 0;
                    DataTable dt = new DataTable();

                    dt.Columns.Add("ID_Operacion");

                    foreach (SelectedRow operacionSeleccionada in rsm.SelectedRows)
                    {
                        dt.Rows.Add();
                        dt.Rows[fila]["ID_Operacion"] = operacionSeleccionada.RecordID;
                        fila++;
                    }

                    LNEfectivale.ModificaOperacionRespuestaRegla(dt, this.Usuario);
                }

                X.Msg.Notify("Cerrar Caso", "Cierre de Caso <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();

                WdwOperacionesFraude.Hide();
                WdwDetalleTarjeta.Hide();

                btnBuscar.FireEvent("click");
            }

            catch (CAppException caEx)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cierre de Caso", "Ocurrió un Error con el Cierre del Caso").Show();
            }

            catch (Exception ex)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cierre de Caso", ex.Message).Show();
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

                StoreResultadosIncidencias.DataSource = DAOEfectivaleOnline.ObtieneIncidenciasPorTarjeta(
                    this.hdnTarjeta.Text, Convert.ToDateTime(this.dfFechaInicialInc.SelectedDate),
                    Convert.ToDateTime(this.dfFechaFinalInc.SelectedDate),
                    String.IsNullOrEmpty(this.cBoxAccionesInc.SelectedItem.Value) ? -1 : 
                        Convert.ToInt32(this.cBoxAccionesInc.SelectedItem.Value), "",
                    String.IsNullOrEmpty(this.cBoxReglasInc.SelectedItem.Value) ? -1 : 
                        Convert.ToInt32(this.cBoxReglasInc.SelectedItem.Value),
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreResultadosIncidencias.DataBind();
            }

            catch (CAppException caEx)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Incidencias", "Ocurrió un Error en la Consulta de Incidencias").Show();
            }

            catch (Exception ex)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
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
            this.cBoxReglasInc.Clear();
            this.cBoxAccionesInc.Clear();

            dfFechaInicialInc.SetValue(DateTime.Today);
            dfFechaFinalInc.SetValue(DateTime.Today);

            this.btnExportExcelInc.Disabled = true;
            this.btnExportCSVInc.Disabled = true;

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

                StoreResultadosOper.DataSource = DAOEfectivaleOnline.ObtieneOperacionesPorTarjeta(
                    this.hdnTarjeta.Text, Convert.ToDateTime(this.dfFechaInicialOper.SelectedDate),
                    Convert.ToDateTime(this.dfFechaFinalOper.SelectedDate),
                    String.IsNullOrEmpty(this.cBoxEstatusOper.SelectedItem.Value) ? -1 :
                    int.Parse(this.cBoxEstatusOper.SelectedItem.Value),
                    this.txtAfiliacionOper.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreResultadosOper.DataBind();
            }

            catch (CAppException caEx)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", "Ocurrió un Error en la Consulta de Operaciones").Show();

            }

            catch (Exception ex)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
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
            this.txtAfiliacionOper.Clear();

            dfFechaInicialOper.SetValue(DateTime.Today);
            dfFechaFinalOper.SetValue(DateTime.Today);

            this.btnExportExcelOper.Disabled = true;
            this.btnExportCSVOper.Disabled = true;

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
        /// Controla el evento Click al botón Marcar Operación, llamando a inserción de la incidencia
        /// en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnMarcar_Click(object sender, EventArgs e)
        {
            try
            {
                string resp = LNEfectivale.MarcaOperacionIncidencia(int.Parse(this.hdnIdOperacion.Text),
                    this.hdnTarjeta.Text, this.txtComentarios.Text, this.Usuario);

                WdwComentarios.Hide();

                if (!String.IsNullOrEmpty(resp))
                {
                    X.Msg.Alert("Operaciones", resp + "<br /> ").Show();
                }
                else
                {
                    X.Msg.Notify("Operaciones", "Marcada como Incidencia <br /><br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                    PreCargaIncidenciasPorTarjeta();
                    btnBuscar.FireEvent("click");
                }
            }

            catch (CAppException caEx)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", "Ocurrió un Error al Marcar la Operación como Incidencia").Show();
            }

            catch (Exception ex)
            {
                DALValidacionesBatchPPF.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de detalle
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreIncidencias_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            Loguear.Evento("Entra a Refresh()", "");

            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPrincipal(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid principal, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPrincipal(int RegistroInicial, string Columna, SortDirection Orden)
        {
            try
            {
                DataTable dtIncidencias = new DataTable();

                dtIncidencias = HttpContext.Current.Session["DtIncidencias"] as DataTable;

                if (dtIncidencias == null)
                {
                    EstadisticasTarjetasEFV estadisticas = new EstadisticasTarjetasEFV();

                    estadisticas.NumTarjeta = txtTarjeta.Text;
                    estadisticas.FechaInicial = Convert.ToDateTime(dfFechaInicial.SelectedDate);
                    estadisticas.FechaFinal = Convert.ToDateTime(dfFechaFinal.SelectedDate);
                    estadisticas.IdAccion = String.IsNullOrEmpty(cmbAccion.SelectedItem.Value) ? -1 :
                        int.Parse(cmbAccion.SelectedItem.Value);
                    estadisticas.IdRegla = String.IsNullOrEmpty(cmbRegla.SelectedItem.Value) ? -1 :
                        int.Parse(cmbRegla.SelectedItem.Value);
                    //estadisticas.NumRegistros = String.IsNullOrEmpty(nmbTop.Text) ? 100 : int.Parse(nmbTop.Text);
                    estadisticas.OperConIncidencia = String.IsNullOrEmpty(cBoxIncidencias.SelectedItem.Value) ? -1 :
                        int.Parse(cBoxIncidencias.SelectedItem.Value);

                    dtIncidencias = DAOEfectivaleOnline.ObtieneIncidenciasVG(estadisticas,
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                    Loguear.Evento("Obtuvo reporte de BD", "");

                    HttpContext.Current.Session.Add("DtIncidencias", dtIncidencias);
                }

                if (dtIncidencias.Rows.Count < 1)
                {
                    X.Msg.Alert("Detalle de Movimientos", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else
                {
                    int TotalRegistros = dtIncidencias.Rows.Count;

                    (this.StoreIncidencias.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtIncidencias.Clone();
                    DataTable dtToGrid = dtIncidencias.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtIncidencias.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingIncidencias.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingIncidencias.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtIncidencias.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreIncidencias.DataSource = dtToGrid;
                    StoreIncidencias.DataBind();

                    //nmbTop.Disabled = false;
                    btnExportExcel.Disabled = false;
                    btnExportCSV.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Búsqueda de Incidencias", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Incidencias", ex.Message).Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar, llamando a la búsqueda de incidencias de tarjetas
        /// en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            //CambiaNumRegistros();

            LimpiaGridIncidencias();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de detalle de la cuenta
        /// </summary>
        protected void LimpiaGridIncidencias()
        {
            btnExportExcel.Disabled = true;
            btnExportCSV.Disabled = true;

            HttpContext.Current.Session.Add("DtIncidencias", null);
            StoreIncidencias.RemoveAll();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //[DirectMethod(Namespace = "ValidacionesBatch")]
        //public void CambiaNumRegistros()
        //{
        //    LimpiaGridIncidencias();

        //    Thread.Sleep(100);

        //    btnBuscarHide.FireEvent("click");
        //}

        /// <summary>
        /// Exporta el reporte de incidencias, previamente consultado, a un archivo Excel 
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportDataTableToExcel(object sender, DirectEventArgs e)
        {
            try
            {
                string reporte = "T";
                string reportName = "Incidencias";
                DataTable _dtIncidencias = HttpContext.Current.Session["DtIncidencias"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtIncidencias, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsColumns(ws, ws.Column(1).CellsUsed().Count(), reporte);

                //Se prepara la respuesta
                this.Response.Clear();
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.Buffer = false;

                this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=" + reportName + ".xlsx");

                //Se envía el reporte como respuesta
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(this.Response.OutputStream);
                    memoryStream.Close();
                }

                this.Response.End();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones e Incidencias", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
            }
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al control de
        /// registros por mostrar
        /// </summary>
        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void StopMask()
        {
            X.Mask.Hide();
        }
    }
}