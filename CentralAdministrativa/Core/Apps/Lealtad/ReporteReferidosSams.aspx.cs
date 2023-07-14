using ClosedXML.Excel;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Utilidades;
using DALLealtad.BaseDatos;
using DALLealtad.Entidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace Lealtad
{
    public partial class ReporteReferidosSams : DALCentralAplicaciones.PaginaBaseCAPP
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
                    dfFechaInicio.SetValue(DateTime.Today);

                    dfFechaFin.MaxDate = DateTime.Today;
                    dfFechaFin.SetValue(DateTime.Today);

                    DataSet dsCadenas = DAOEcommercePrana.ListaCadenas(this.Usuario);
                    List<CadenaComboPredictivo> ListaCadenas = new List<CadenaComboPredictivo>();

                    foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
                    {
                        var ComboCadenas = new CadenaComboPredictivo()
                        {
                            ID_Cadena = Convert.ToInt64(cadena["ID_Cadena"].ToString()),
                            ClaveCadena = cadena["ClaveCadena"].ToString(),
                            NombreComercial = cadena["NombreComercial"].ToString()
                        };
                        ListaCadenas.Add(ComboCadenas);
                    }

                    StoreCadena.DataSource = ListaCadenas;
                    StoreCadena.DataBind();

                    PagingToolBar1.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtCupones", null);
                }

                if (!X.IsAjaxRequest)
                {
                }
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, "");
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
            dfFechaInicio.SetValue(DateTime.Today);
            dfFechaFin.SetValue(DateTime.Today);
            cBoxCadena.Reset();
        
            LimpiaGridCupones();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al gid de cupones
        /// </summary>
        protected void LimpiaGridCupones()
        {
            this.btnExportExcel.Disabled = true;
            this.btnExportCSV.Disabled = true;

            HttpContext.Current.Session.Add("DtCupones", null);
            StoreCupones.RemoveAll();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de cupones, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelCupones(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;
            this.btnExportCSV.Disabled = true;

            if ((dfFechaInicio.SelectedDate == DateTime.MinValue) ||
                (dfFechaFin.SelectedDate == DateTime.MinValue))
            {
                X.Mask.Hide();
                return;
            }

            try
            {
                DataTable dtCupones = new DataTable();

                dtCupones = HttpContext.Current.Session["DtCupones"] as DataTable;

                if (dtCupones == null)
                {
                    dtCupones = DAOPromociones.ObtieneReporteReferidosSams(
                        Convert.ToDateTime(this.dfFechaInicio.SelectedDate),
                        Convert.ToDateTime(this.dfFechaFin.SelectedDate),
                        String.IsNullOrEmpty(cBoxCadena.SelectedItem.Value) ? "-1" :
                            cBoxCadena.SelectedItem.Value,
                        this.Usuario);

                    HttpContext.Current.Session.Add("DtCupones", dtCupones);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtCupones.Rows.Count < 1)
                {
                    X.Msg.Alert("Cupones Referidos", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtCupones.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Cupones Referidos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "Lealtad.ClicDePaso()",
                                Text = "Aceptar"
                            },
                            No = new MessageBoxButtonConfig
                            {
                                Text = "Cancelar"
                            }
                        }).Show();
                }
                else
                {
                    int TotalRegistros = dtCupones.Rows.Count;
                    
                    (this.StoreCupones.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtCupones.Clone();
                    DataTable dtToGrid = dtCupones.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtCupones.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingToolBar1.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtCupones.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreCupones.DataSource = dtToGrid;
                    StoreCupones.DataBind();

                    this.btnExportExcel.Disabled = false;
                    this.btnExportCSV.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cupones Referidos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cupones Referidos", "Ocurrió un Error al Consultar los Cupones Referidos").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, DirectEventArgs e)
        {
            LimpiaGridCupones();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
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
        /// Controla el evento onRefresh del grid de cupones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreCupones_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelCupones(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "Lealtad")]
        public void ClicDePaso()
        {
            btnDownloadHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void Download(object sender, DirectEventArgs e)
        {
            ExportDataTableToExcel();
        }

        /// <summary>
        /// Exporta el reporte previamento consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "Lealtad")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteCuponesReferidos";
                DataTable _dtCupones = HttpContext.Current.Session["DtCupones"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtCupones, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsColumns(ws, ws.Column(1).CellsUsed().Count());

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
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cupones Referidos", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
            }
        }

        /// <summary>
        /// Establece el formato deseado a las columnas de la hoja de trabajo por exportar
        /// </summary>
        /// <param name="ws">Hoja de trabajo</param>
        /// <param name="rowsNum">Total de filas de la hoja de trabajo</param>
        /// <returns></returns>
        protected IXLWorksheet FormatWsColumns(IXLWorksheet ws, int rowsNum)
        {
            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
            }

            return ws;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "Lealtad")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}