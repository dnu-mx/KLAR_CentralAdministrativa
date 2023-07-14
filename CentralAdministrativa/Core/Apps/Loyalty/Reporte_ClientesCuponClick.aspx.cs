using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.BaseDatos;
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

namespace CentroContacto
{
    public partial class Reporte_ClientesCuponClick : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //Se prestablecen las fechas de consulta del reporte
                    datInicio.SetValue(DateTime.Today);
                    datInicio.MaxDate = DateTime.Today;

                    datFinal.SetValue(DateTime.Today);
                    datFinal.MaxDate = DateTime.Today;

                    PagingToolBar1.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtMovsClientesCuponClick", null);
                }
            }

            catch (Exception err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        

        /// <summary>
        /// Controla la alimentación de datos del grid de movimientos, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelMovimientos(int RegistroInicial, string Columna, SortDirection Orden)
        {
            btnExportExcel.Disabled = true;
            btnExportCSV.Disabled = true;
            
            try
            {
                DataTable dtMovsCuponClick = new DataTable();

                dtMovsCuponClick = HttpContext.Current.Session["DtMovsClientesCuponClick"] as DataTable;

                if (dtMovsCuponClick == null)
                {
                    dtMovsCuponClick = DAOReportes.ObtenerReporteClientesCuponClick(datInicio.SelectedDate,
                        datFinal.SelectedDate, txtNombre.Text, txtApPaterno.Text, txtApMaterno.Text, 
                        txtCorreo.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                    HttpContext.Current.Session.Add("DtMovsClientesCuponClick", dtMovsCuponClick);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtMovsCuponClick.Rows.Count < 1)
                {
                    X.Msg.Alert("Reporte de Clientes CuponClick", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtMovsCuponClick.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Reporte de Clientes CuponClick", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepClientesCuponClick.ClicDePaso()",
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
                    int TotalRegistros = dtMovsCuponClick.Rows.Count;

                    (this.StoreMovimientos.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtMovsCuponClick.Clone();
                    DataTable dtToGrid = dtMovsCuponClick.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtMovsCuponClick.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingToolBar1.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtMovsCuponClick.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreMovimientos.DataSource = dtToGrid;
                    StoreMovimientos.DataBind();

                    btnExportExcel.Disabled = false;
                    btnExportCSV.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Reporte de Clientes CuponClick", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Reporte de Clientes CuponClick", "Ocurrió un Error al Consultar el Reporte").Show();
            }

            finally
            {
                X.Mask.Hide();
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
        /// Controla el evento Click al botón Buscar, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridMovimientos();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar, restableciendo los controles
        /// a su estatus de carga inicial de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            datInicio.Reset();
            datFinal.Reset();
            txtNombre.Reset();
            txtApPaterno.Reset();
            txtApMaterno.Reset();
            txtCorreo.Reset();

            LimpiaGridMovimientos();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al Grid de Movimientos
        /// </summary>
        protected void LimpiaGridMovimientos()
        {
            btnExportExcel.Disabled = true;
            btnExportCSV.Disabled = true;

            HttpContext.Current.Session.Add("DtMovsClientesCuponClick", null);
            StoreMovimientos.RemoveAll();
        }
        
        /// <summary>
        /// Controla el evento onRefresh del grid de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreMovimientos_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelMovimientos(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepClientesCuponClick")]
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
        [DirectMethod(Namespace = "RepClientesCuponClick")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteClientesCuponClick";
                DataTable _dtMovimientosCash = HttpContext.Current.Session["DtMovsClientesCuponClick"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtMovimientosCash, reportName);

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
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Reporte de Clientes CuponClick", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 9).SetDataType(XLCellValues.DateTime);
            }

            return ws;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepClientesCuponClick")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}