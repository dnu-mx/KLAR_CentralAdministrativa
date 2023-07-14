using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALValidacionesBatchPPF.BaseDatos;
using DALValidacionesBatchPPF.Entidades;
using DALValidacionesBatchPPF.LogicaNegocio;
using DALValidacionesBatchPPF.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Xsl;


namespace ValidacionesBatch
{
    public partial class ConsultaTarjetasVG : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Consulta Tarjetas Efectivale
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

                    EstableceRolesBloqueo();
                    
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
            dfFechaInicial.MaxDate = DateTime.Today;
            dfFechaInicial.MinDate = DateTime.Today.AddDays(-180);
            dfFechaInicial.SetValue(DateTime.Today);

            dfFechaFinal.MaxDate = DateTime.Today;
            dfFechaFinal.SetValue(DateTime.Today);

            //Se consulta el catálogo de Acciones y se establece en ambos controles
            this.StoreAcciones.DataSource = DAOEfectivaleOnline.ListaAcciones(
                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
            this.StoreAcciones.DataBind();

            //Se consulta el catálogo de Reglas y se establece en ambos controles
            this.StoreReglas.DataSource = DAOEfectivaleOnline.ListaReglas(
                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
            this.StoreReglas.DataBind();
        }

        /// <summary>
        /// Realiza el pre llenado de los controles de la pantalla secundaria
        /// </summary>
        protected void PreLoadFrameScreenControls()
        {
            //Se establecen valores máximos para los controles de fecha
            dfFechaInicialOper.MaxDate = DateTime.Today;
            dfFechaFinalOper.MaxDate = DateTime.Today;

            dfFechaInicialInc.MaxDate = DateTime.Today;
            dfFechaInicialInc.MinDate = DateTime.Today.AddDays(-180);
            dfFechaFinalInc.MaxDate = DateTime.Today;
        }

        /// <summary>
        /// Establece en variables de sesión los roles que pueden bloquear y/o
        /// desbloquear una tarjeta
        /// </summary>
        protected void EstableceRolesBloqueo()
        {
            this.Usuario.Roles.Sort();
            HttpContext.Current.Session.Add("EsAdmin", false);
            HttpContext.Current.Session.Add("EsAnalista", false);

            foreach (string rol in this.Usuario.Roles)
            {
                if (rol.ToUpper().Contains("SUPERV") || rol.ToUpper().Contains("ADMIN"))
                {
                    HttpContext.Current.Session.Add("EsAdmin", true);
                }
                else if (rol.ToUpper().Contains("ANALIST"))
                {
                    HttpContext.Current.Session.Add("EsAnalista", true);
                }
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar, invocando a la 
        /// búsqueda de tarjetas en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                StoreTarjetas.RemoveAll();

                if (!String.IsNullOrEmpty(this.txtTarjeta.Text))
                {
                    Int64 tarjeta;

                    if (!Int64.TryParse(this.txtTarjeta.Text, out tarjeta))
                    {
                        X.Msg.Alert("Filtros de búsqueda", "La Tarjeta debe ser numérica").Show();
                        return;
                    }
                }

                EstadisticasTarjetasEFV estadisticas = new EstadisticasTarjetasEFV();

                estadisticas.NumTarjeta = this.txtTarjeta.Text;
                estadisticas.FechaInicial = Convert.ToDateTime(this.dfFechaInicial.SelectedDate);
                estadisticas.FechaFinal = Convert.ToDateTime(this.dfFechaFinal.SelectedDate);

                for (int iSelectedItem = 0; iSelectedItem < mCmbAccion.SelectedItems.Count; iSelectedItem++)
                {
                    estadisticas.Acciones += mCmbAccion.SelectedItems[iSelectedItem].Value + ";";
                }

                estadisticas.IdRegla = String.IsNullOrEmpty(this.cBoxRegla.SelectedItem.Value) ? -1 : 
                    int.Parse(this.cBoxRegla.SelectedItem.Value);
                estadisticas.NumRegistros = String.IsNullOrEmpty(this.nmbTop.Text) ? 100 : int.Parse(nmbTop.Text);

                DataSet dsEstadisticas = DAOEfectivaleOnline.ObtieneEstadisticasTarjeta(estadisticas,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsEstadisticas.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Tarjetas", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    this.nmbTop.Disabled = false;
                    this.btnExportExcel.Disabled = false;
                    this.btnExportCSV.Disabled = false;

                    StoreTarjetas.DataSource = dsEstadisticas;
                    StoreTarjetas.DataBind();
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Tarjetas", "Ocurrió un Error en la Consulta de Tarjetas").Show();

            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Tarjetas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar,
        /// restableciendo los controles de la página al valor de carga origen
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            FormPanelMain.Reset();

            dfFechaInicial.SetValue(DateTime.Today);
            dfFechaFinal.SetValue(DateTime.Today);
            this.txtTarjeta.Reset();

            this.mCmbAccion.Reset();
            this.cBoxRegla.Reset();

            this.nmbTop.Reset();
            this.nmbTop.Disabled = true;

            this.btnExportExcel.Disabled = true;
            this.btnExportCSV.Disabled = true;

            StoreTarjetas.RemoveAll();
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
            string reportName = 
                reporte == "T" ? "Tarjetas" : reporte == "O" ? "Operaciones" : "Incidencias";

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
                        ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                        break;

                    case "O":
                        ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                        ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
                        break;

                    case "I":
                        ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
                        ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                        break;
                }
            }

            return ws;
        }

        /// <summary>
        /// Controla el evento SUBMIT al querer exportar al formato seleccionado
        /// los resultados de la consulta de operaciones
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
        /// Da una pausa al sistema para mostrar y finaliza la máscara al control de
        /// registros por mostrar
        /// </summary>
        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void StopMask()
        {
            X.Mask.Hide();
        }

        /// <summary>
        /// Controla el evento de selección de una celda del grid de Operaciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void CellTarjetas_Click(object sender, DirectEventArgs e)
        {
            CellSelectionModel sm = this.GridTarjetas.SelectionModel.Primary as CellSelectionModel;

            if (String.Compare(sm.SelectedCell.Name, "NumTarjeta") != 0)
            {
                return;
            }

            else
            {
                if (!String.IsNullOrEmpty(sm.SelectedCell.Value))
                {
                    WdwDetalleTarjeta.Title = "Tarjeta: " + sm.SelectedCell.Value;
                    this.hdnTarjeta.Text = sm.SelectedCell.Value;

                    InitChkBloqTrajeta();                    
                    InitTabOperaciones();
                    InitTabIncidencias();

                    this.WdwDetalleTarjeta.Show();
                    FormPanelOperaciones.Show();
                }

            }
        }

        /// <summary>
        /// Inicializa el CheckBox de bloqueo de tarjeta según su valor en base
        /// de datos y el rol del usuario en sesión
        /// </summary>
        protected void InitChkBloqTrajeta()
        {
            Boolean esAdministrador = Boolean.Parse(HttpContext.Current.Session["EsAdmin"].ToString());
            Boolean esAnalista = Boolean.Parse(HttpContext.Current.Session["EsAnalista"].ToString());

             DataSet dsEstatus = DAOEfectivaleOnline.ObtieneEstatusTarjeta(
                        this.hdnTarjeta.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

            if (dsEstatus.Tables[0].Rows.Count > 0)
            {
                string EstatusTarjeta = dsEstatus.Tables[0].Rows[0]["EstatusMA"].ToString().Trim();
                int IdBloqTarjeta = int.Parse(dsEstatus.Tables[0].Rows[0]["IdBloqMA"].ToString().Trim());

                lblEstatus.Text = "Estatus: " + EstatusTarjeta;

                chkBxBloquear.Checked = Convert.ToBoolean(IdBloqTarjeta);
                if (EstatusTarjeta.ToUpper().Contains("CANCEL"))
                {
                    chkBxBloquear.Disabled = true;
                }
                else
                {
                    chkBxBloquear.Disabled = esAdministrador ? false : esAnalista && !chkBxBloquear.Checked ? false : true;
                }
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
        /// Inicializa la pestaña de Incidencias según los valores seleccionados
        /// de la pantalla de consulta de tarjetas.
        /// </summary>
        protected void InitTabIncidencias()
        {
            LimpiaTabIncidencias();

            dfFechaInicialInc.SetValue(dfFechaInicial.Value);
            dfFechaFinalInc.SetValue(dfFechaFinal.Value);

            for (int iSelectedItem = 0; iSelectedItem < mCmbAccion.SelectedItems.Count; iSelectedItem++)
            {
                mCmbAccionInc.SelectItem(mCmbAccion.SelectedItems[iSelectedItem].Value);
            }
            
            cBoxReglasInc.Value = cBoxRegla.Value;

            PreCargaIncidenciasPorTarjeta();
        }

        /// <summary>
        /// Realiza las validaciones para bloquear o desbloquear la tarjeta
        /// </summary>
        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void BloqueaDesbloqueaTarjeta()
        {
            string accion = chkBxBloquear.Checked ? "Bloqueo" : "Desbloqueo";
            string titulo = chkBxBloquear.Checked ? "Bloquear" : "Desbloquear";

            try
            {
                if (chkBxBloquear.Checked)
                {
                    LNEfectivale.BloqueaTarjeta(this.hdnTarjeta.Text, this.Usuario);
                }
                else
                {
                    LNEfectivale.DesbloqueaTarjeta(this.hdnTarjeta.Text, this.Usuario);
                }

                InitChkBloqTrajeta();
                X.Msg.Notify(titulo, accion + " de Tarjeta <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert(titulo, "Ocurrió un Error con el " + accion + " de la Tarjeta").Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert(titulo, ex.Message).Show();
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
                if (!String.IsNullOrEmpty(this.txtComercioOper.Text))
                {
                    Int64 estac;

                    if (!Int64.TryParse(this.txtComercioOper.Text, out estac))
                    {
                        X.Msg.Alert("", "El Comercio debe ser numérico").Show();
                        return;
                    }
                }

                StoreResultadosOper.RemoveAll();

                DataSet dsOperaciones = new DataSet();
                //DataSet dsOperaciones = DAOEfectivaleOnline.ObtieneOperacionesPorTarjeta(
                //    this.hdnTarjeta.Text,
                //    Convert.ToDateTime(this.dfFechaInicialOper.SelectedDate),
                //    Convert.ToDateTime(this.dfFechaFinalOper.SelectedDate),
                //    this.txtComercioOper.Text, this.Usuario,
                //    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsOperaciones.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Operaciones", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreResultadosOper.DataSource = dsOperaciones;
                    StoreResultadosOper.DataBind();

                    this.btnExportExcelOper.Disabled = false;
                    this.btnExportCSVOper.Disabled = false;
                }
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

            dfFechaInicialOper.SetValue(DateTime.Today);
            dfFechaFinalOper.SetValue(DateTime.Today);
            this.txtComercioOper.Reset();

            this.btnExportExcelOper.Disabled = true;
            this.btnExportCSVOper.Disabled = true;

            StoreResultadosOper.RemoveAll();
        }

        /// <summary>
        /// Realiza y carga en el Grid de Operaciones una pre consulta de las operaciones 
        /// de la tarjeta seleccionada el día de hoy
        /// </summary>
        protected void PreCargaOperaciones()
        {
            try
            {
                DataSet dsOperaciones = new DataSet();

                //DataSet dsOperaciones = DAOEfectivaleOnline.ObtieneOperacionesPorTarjeta(
                //     this.hdnTarjeta.Text,
                //     Convert.ToDateTime(this.dfFechaInicialOper.SelectedDate),
                //     Convert.ToDateTime(this.dfFechaFinalOper.SelectedDate),
                //     this.txtComercioOper.Text, this.Usuario,
                //     Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsOperaciones.Tables[0].Rows.Count > 0)
                {
                    StoreResultadosOper.DataSource = dsOperaciones;
                    StoreResultadosOper.DataBind();

                    this.btnExportExcelOper.Disabled = false;
                    this.btnExportCSVOper.Disabled = false;
                }
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
        /// Controla el evento Click al botón Buscar de la pestaña de Incidencias, invocando a la 
        /// búsqueda de operaciones de la tarjeta en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscaInc_Click(object sender, EventArgs e)
        {
            try
            {
                String acciones = "";

                for (int iSelectedItem = 0; iSelectedItem < mCmbAccion.SelectedItems.Count; iSelectedItem++)
                {
                    acciones += mCmbAccionInc.SelectedItems[iSelectedItem].Value + ";";
                }

                StoreResultadosIncidencias.RemoveAll();
                DataSet dsIncidencias = new DataSet();
                //DataSet dsIncidencias = DAOEfectivaleOnline.ObtieneIncidenciasPorTarjeta(
                //    this.hdnTarjeta.Text,
                //    Convert.ToDateTime(this.dfFechaInicialInc.SelectedDate), Convert.ToDateTime(this.dfFechaFinalInc.SelectedDate),
                //    acciones,
                //    String.IsNullOrEmpty(this.cBoxReglasInc.SelectedItem.Value) ? -1 : Convert.ToInt32(this.cBoxReglasInc.SelectedItem.Value),
                //     this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsIncidencias.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Incidencias", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreResultadosIncidencias.DataSource = dsIncidencias;
                    StoreResultadosIncidencias.DataBind();

                    this.btnExportExcelInc.Disabled = false;
                    this.btnExportCSVInc.Disabled = false;
                }
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

            dfFechaInicialInc.SetValue(DateTime.Today);
            dfFechaFinalInc.SetValue(DateTime.Today);

            this.mCmbAccionInc.Reset();
            this.cBoxReglasInc.Reset();

            this.btnExportExcelInc.Disabled = true;
            this.btnExportCSVInc.Disabled = true;

            StoreResultadosIncidencias.RemoveAll();
        }

        /// <summary>
        /// Realiza y carga en el Grid Resultados de Incidencias una pre consulta
        /// de ellas de la tarjeta seleccionada en los últimos 6 meses
        /// </summary>
        protected void PreCargaIncidenciasPorTarjeta()
        {
            try
            {
                String acciones = "";

                for (int iSelectedItem = 0; iSelectedItem < mCmbAccionInc.SelectedItems.Count; iSelectedItem++)
                {
                    acciones += mCmbAccionInc.SelectedItems[iSelectedItem].Value + ";";
                }

                DataSet dsIncidencias = DAOEfectivaleOnline.ObtieneIncidenciasPorTarjeta(
                    this.hdnTarjeta.Text, Convert.ToDateTime(this.dfFechaInicialInc.SelectedDate),
                    Convert.ToDateTime(this.dfFechaFinalInc.SelectedDate), -1, acciones,
                    String.IsNullOrEmpty(this.cBoxReglasInc.SelectedItem.Value) ? -1 : Convert.ToInt32(this.cBoxReglasInc.SelectedItem.Value),
                     this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsIncidencias.Tables[0].Rows.Count > 0)
                {
                    StoreResultadosIncidencias.DataSource = dsIncidencias;
                    StoreResultadosIncidencias.DataBind();

                    this.btnExportExcelInc.Disabled = false;
                    this.btnExportCSVInc.Disabled = false;
                }
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
    }
}