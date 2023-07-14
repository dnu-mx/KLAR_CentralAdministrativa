using ClosedXML.Excel;
using DALValidacionesBatchPPF.BaseDatos;
using DALValidacionesBatchPPF.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;

namespace ValidacionesBatch
{
    public partial class Reporte_CasosEFV : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DateTime fechaHoy = DateTime.Today;

                    dfFechaInicio.MinDate = fechaHoy.AddDays(-90);
                    dfFechaInicio.SetValue(new DateTime(fechaHoy.Year, fechaHoy.Month, 1));

                    dfFechaFin.MaxDate = fechaHoy;
                    dfFechaFin.SetValue(fechaHoy);

                    StoreDictamen.DataSource = DAOEfectivaleOnline.ListaDictamenes(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreDictamen.DataBind();

                    StoreTipoFraude.DataSource = DAOEfectivaleOnline.ListaTiposFraude(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreTipoFraude.DataBind();
                }

                if (!X.IsAjaxRequest)
                {

                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            FormPanelFiltros.Reset();
            txtTarjeta.Clear();
            DateTime fechaHoy = DateTime.Today;
            dfFechaInicio.SetValue(new DateTime(fechaHoy.Year, fechaHoy.Month, 1));
            dfFechaFin.SetValue(fechaHoy);
            cBoxDictamen.ClearValue();
            cBoxTipoFraude.ClearValue();

            StoreCasos.RemoveAll();
            btnExportExcel.Disabled = true;
            btnExportCSV.Disabled = true;

            StoreResultadosIncidencias.RemoveAll();
            btnExportExcelInc.Disabled = true;
            btnExportCSVInc.Disabled = true;
        }

        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            this.btnExportExcel.Disabled = true;
            this.btnExportCSV.Disabled = true;

            if ((dfFechaInicio.SelectedDate == DateTime.MinValue) ||
                (dfFechaFin.SelectedDate == DateTime.MinValue))
                return;

            try
            {
                DataSet dsCasos = DAOReportes.ReporteCasosEFV(this.txtTarjeta.Text,
                    Convert.ToDateTime(this.dfFechaInicio.SelectedDate),
                    Convert.ToDateTime(this.dfFechaFin.SelectedDate),
                    String.IsNullOrEmpty(cBoxDictamen.SelectedItem.Value) ? -1 : 
                    int.Parse(cBoxDictamen.SelectedItem.Value),
                    String.IsNullOrEmpty(cBoxTipoFraude.SelectedItem.Value) ? -1 :
                    int.Parse(cBoxTipoFraude.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsCasos.Tables[0].Rows.Count < 1)
                {
                    X.Msg.Alert("Casos", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else
                {
                    StoreCasos.DataSource = dsCasos;
                    StoreCasos.DataBind();

                    this.btnExportExcel.Disabled = false;
                    this.btnExportCSV.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Casos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Casos", "Ocurrió un Error al Consultar el Reporte de Casos").Show();
            }
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
                reporte == "C" ? "Casos" : "IncidenciasPorCaso";

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
                    case "C":
                        ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
                        ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.DateTime);
                        break;

                    //case "I":
                    default:
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
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void StopMask()
        {
            X.Mask.Hide();
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
        /// Controla el evento de selección de una celda del grid de Incidencias
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void CellGridCasos_Click(object sender, DirectEventArgs e)
        {
            CellSelectionModel sm = GridCasos.SelectionModel.Primary as CellSelectionModel;

            if (String.Compare(sm.SelectedCell.Name, "ID_Caso") != 0)
            {
                return;
            }

            if (!String.IsNullOrEmpty(sm.SelectedCell.Value))
            {
                PreCargaIncidencias(Convert.ToInt64(sm.SelectedCell.Value));

                this.WdwDetalleTarjeta.Show();
                FormPanelIncidencias.Show();
            }
        }

        /// <summary>
        /// Realiza y carga en el Grid Resultados de Incidencias una pre consulta
        /// de ellas de la tarjeta seleccionada en el periodo seleccionado
        /// </summary>
        protected void PreCargaIncidencias(Int64 IdCaso)
        {
            try
            {
                DataSet dsIncidencias = 
                    DAOReportes.ObtieneIncidenciasPorCaso(IdCaso, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsIncidencias.Tables[0].Rows.Count > 0)
                {
                    this.btnExportExcelInc.Disabled = false;
                    this.btnExportCSVInc.Disabled = false;

                    string tarjeta = "Tarjeta: " + dsIncidencias.Tables[0].Rows[0]["Tarjeta"].ToString().Trim();
                    string estatus = dsIncidencias.Tables[0].Rows[0]["EstatusTarjeta"].ToString().Trim();
                    string caso = "No. Caso: " + IdCaso.ToString();

                    WdwDetalleTarjeta.Title = caso +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       tarjeta + " - " + estatus;

                    //string estatus = dsIncidencias.Tables[0].Rows[0]["EstatusTarjeta"].ToString().Trim();

                    //lblEstatusTarjeta.Text += estatus;

                    StoreResultadosIncidencias.DataSource = dsIncidencias;
                    StoreResultadosIncidencias.DataBind();
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
    }
}