using ClosedXML.Excel;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Utilidades;
using DALLealtad.BaseDatos;
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
    public partial class ReporteCuponesRedimidos : DALCentralAplicaciones.PaginaBaseCAPP
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

                    StoreTipoEmision.DataSource = DAOEcommercePrana.ListaTiposEmision(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreTipoEmision.DataBind();

                    DataSet dsCadenas = DAOPromociones.ObtieneColectivasParaFiltros(-1,
                        Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "ClaveTipoColectivaCadenaComercial").Valor, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                    List<ColectivaComboPredictivo> ListaCadenas = new List<ColectivaComboPredictivo>();

                    foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
                    {
                        var ComboCadenas = new ColectivaComboPredictivo()
                        {
                            ID_Colectiva = Convert.ToInt64(cadena["ID_Colectiva"].ToString()),
                            ClaveColectiva = cadena["ClaveColectiva"].ToString(),
                            NombreORazonSocial = cadena["NombreORazonSocial"].ToString()
                        };
                        ListaCadenas.Add(ComboCadenas);
                    }

                    StoreCadena.DataSource = ListaCadenas;
                    StoreCadena.DataBind();

                    PagingToolBar1.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtRedenciones", null);
                }

                if (!X.IsAjaxRequest)
                {
                    X.Mask.Show(new MaskConfig { Msg = "Obteniendo Cupones Redimidos..." });
                }
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Carga los catálogos de sucursales y promociones de la cadena comercial elegida,
        /// en los controles destinados a ello, con la información de base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void LlenaSucursalesYPromociones(object sender, EventArgs e)
        {
            try
            {
                StoreSucursal.RemoveAll();
                StorePromocion.RemoveAll();

                StoreSucursal.DataSource = DAOPromociones.ObtieneColectivasParaFiltros(
                    Int64.Parse(cBoxCadena.SelectedItem.Value.ToString()), "SUC", this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreSucursal.DataBind();              

                StorePromocion.DataSource = DAOPromociones.ObtienePromocionesCuponesDeCadena(
                    Convert.ToInt64(cBoxCadena.SelectedItem.Value), this.Usuario);
                StorePromocion.DataBind();
            }

            catch (Exception err)
            {
                X.Msg.Alert("Detalle Redenciones", err.Message).Show();
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
            cBoxPromocion.Reset();
            cBoxTipoEmision.Reset();
            cBoxCadena.Reset();
            cBoxSucursal.Reset();

            LimpiaGridRedenciones();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al gid de redenciones
        /// </summary>
        protected void LimpiaGridRedenciones()
        {
            this.btnExportExcel.Disabled = true;
            this.btnExportCSV.Disabled = true;

            HttpContext.Current.Session.Add("DtRedenciones", null);
            StoreRedenciones.RemoveAll();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de redenciones, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelRedenciones(int RegistroInicial, string Columna, SortDirection Orden)
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
                DataTable dtRedenciones = new DataTable();

                dtRedenciones = HttpContext.Current.Session["DtRedenciones"] as DataTable;

                if (dtRedenciones == null)
                {
                    dtRedenciones = DAOPromociones.ObtieneReporteCuponesRedimidos(
                        Convert.ToDateTime(this.dfFechaInicio.SelectedDate),
                        Convert.ToDateTime(this.dfFechaFin.SelectedDate),
                        String.IsNullOrEmpty(cBoxCadena.SelectedItem.Value) ? -1 :
                            Convert.ToInt64(cBoxCadena.SelectedItem.Value),
                        String.IsNullOrEmpty(cBoxSucursal.SelectedItem.Value) ? -1 :
                            Convert.ToInt64(cBoxSucursal.SelectedItem.Value),
                        String.IsNullOrEmpty(cBoxPromocion.SelectedItem.Value) ? -1 :
                            Convert.ToInt32(cBoxPromocion.SelectedItem.Value),
                        String.IsNullOrEmpty(cBoxTipoEmision.SelectedItem.Value) ? -1 :
                            Convert.ToInt32(cBoxTipoEmision.SelectedItem.Value),
                        this.Usuario);

                    HttpContext.Current.Session.Add("DtRedenciones", dtRedenciones);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtRedenciones.Rows.Count < 1)
                {
                    X.Msg.Alert("Redenciones", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtRedenciones.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Redenciones", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
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
                    int TotalRegistros = dtRedenciones.Rows.Count;
                    
                    (this.StoreRedenciones.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtRedenciones.Clone();
                    DataTable dtToGrid = dtRedenciones.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtRedenciones.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingToolBar1.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtRedenciones.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreRedenciones.DataSource = dtToGrid;
                    StoreRedenciones.DataBind();

                    this.btnExportExcel.Disabled = false;
                    this.btnExportCSV.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Redenciones", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Redenciones", "Ocurrió un Error al Consultar el Detalle de Redenciones").Show();
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
            LimpiaGridRedenciones();

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
        /// Controla el evento onRefresh del grid de redenciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreRedenciones_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelRedenciones(inicio, columna, orden);
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
                string reportName = "ReporteRedenciones";
                DataTable _dtRedenciones = HttpContext.Current.Session["DtRedenciones"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtRedenciones, reportName);

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
                X.Msg.Alert("Redenciones", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
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