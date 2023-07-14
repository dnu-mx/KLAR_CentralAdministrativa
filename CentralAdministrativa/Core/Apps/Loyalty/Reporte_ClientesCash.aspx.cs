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
    public partial class Reporte_ClientesCash : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    StoreNivelLealtad.DataSource = DAOReportes.ObtieneCatalogoNivelesLealtadCash();
                    StoreNivelLealtad.DataBind();

                    StoreEstatusColectiva.DataSource = DAOReportes.ObtieneCatalogoEstatusColectiva(
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    StoreEstatusColectiva.DataBind();

                    datInicio.SetValue(DateTime.Now);
                    datInicio.MaxDate = DateTime.Today;

                    datFinal.SetValue(DateTime.Now);
                    datFinal.MaxDate = DateTime.Today;

                    PagingToolBar1.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtClientesCash", null);
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de clientes, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelClientes(int RegistroInicial, string Columna, SortDirection Orden)
        {
            btnExportExcel.Disabled = true;
            btnExportCSV.Disabled = true;

            try
            {
                DataTable dtClientesCash = new DataTable();

                dtClientesCash = HttpContext.Current.Session["DtClientesCash"] as DataTable;

                if (dtClientesCash == null)
                { 
                    dtClientesCash = DAOReportes.ObtenerReporteClientesCash(datInicio.SelectedDate, 
                        datFinal.SelectedDate, nfTelefono.Text,
                        String.IsNullOrEmpty(cBoxNivelLealtad.SelectedItem.Value) ? -1 :
                        int.Parse(cBoxNivelLealtad.SelectedItem.Value),
                        String.IsNullOrEmpty(cBoxEstatusUsuario.SelectedItem.Value) ? -1 :
                        int.Parse(cBoxEstatusUsuario.SelectedItem.Value), this.Usuario);
                    
                    HttpContext.Current.Session.Add("DtClientesCash", dtClientesCash);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtClientesCash == null)
                {
                    X.Msg.Alert("Reporte de Clientes", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtClientesCash.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Reporte de Clientes", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepClientesCash.ClicDePaso()",
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
                    int TotalRegistros = dtClientesCash.Rows.Count;

                    (this.StoreClientes.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtClientesCash.Clone();
                    DataTable dtToGrid = dtClientesCash.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtClientesCash.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingToolBar1.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtClientesCash.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreClientes.DataSource = dtToGrid;
                    StoreClientes.DataBind();

                    btnExportExcel.Disabled = false;
                    btnExportCSV.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Reporte de Clientes", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Reporte de Clientes", "Ocurrió un Error al Consultar el Reporte").Show();
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
            LimpiaGridClientes();

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
            nfTelefono.Reset();
            cBoxNivelLealtad.Reset();
            cBoxEstatusUsuario.Reset();

            LimpiaGridClientes();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de clientes
        /// </summary>
        protected void LimpiaGridClientes()
        {
            btnExportExcel.Disabled = true;
            btnExportCSV.Disabled = true;

            HttpContext.Current.Session.Add("DtClientesCash", null);
            StoreClientes.RemoveAll();
        }

        /// Controla el evento onRefresh del grid de clientes
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreClientes_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelClientes(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepClientesCash")]
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
        [DirectMethod(Namespace = "RepClientesCash")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteClientes";
                DataTable _dtClientesCash = HttpContext.Current.Session["DtClientesCash"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtClientesCash, reportName);

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
                X.Msg.Alert("Reporte de Clientes", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
            ws.Column(1).Hide();

            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 12).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 13).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 15).SetDataType(XLCellValues.Number);
            }

            return ws;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepClientesCash")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}