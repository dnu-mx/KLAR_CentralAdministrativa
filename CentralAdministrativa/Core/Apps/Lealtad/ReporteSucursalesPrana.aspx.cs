using ClosedXML.Excel;
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
    public partial class ReporteSucursalesPrana : DALCentralAplicaciones.PaginaBaseCAPP
    {

        public class CadenaComboPredictivo
        {
            public Int64 ID_Cadena { get; set; }
            public String ClaveCadena { get; set; }
            public String NombreComercial { get; set; }
        }

        public class SucursalComboPredictivo
        {
            public Int64 id_sucursal { get; set; }
            public String clave { get; set; }
            public String nombre { get; set; }
        }

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
                    DataSet dsCadenas = DAOEcommercePrana.ListaCadenas(this.Usuario);
                    
                    List<CadenaComboPredictivo> ComboList = new List<CadenaComboPredictivo>();

                    foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
                    {
                        var cadenaCombo = new CadenaComboPredictivo()
                        {
                            ID_Cadena = Convert.ToInt32(cadena["ID_Cadena"].ToString()),
                            ClaveCadena = cadena["ClaveCadena"].ToString(),
                            NombreComercial = cadena["NombreComercial"].ToString()
                        };
                        ComboList.Add(cadenaCombo);
                    }

                    StoreClaveCadena.DataSource = ComboList;
                    StoreClaveCadena.DataBind();


                    DataSet dsSucursales = DAOEcommercePrana.ListaSucursales(this.Usuario);

                    List<SucursalComboPredictivo> sucursalesList = new List<SucursalComboPredictivo>();

                    foreach (DataRow sucursal in dsSucursales.Tables[0].Rows)
                    {
                        var sucursalCombo = new SucursalComboPredictivo()
                        {
                            id_sucursal = Convert.ToInt32(sucursal["id_sucursal"].ToString()),
                            clave = sucursal["clave"].ToString(),
                            nombre = sucursal["nombre"].ToString()
                        };
                        sucursalesList.Add(sucursalCombo);
                    }

                    StoreClaveSucursal.DataSource = sucursalesList;
                    StoreClaveSucursal.DataBind();

                    StorePais.DataSource = DAOEcommercePrana.ListaPaises(this.Usuario);
                    StorePais.DataBind();

                    StoreEstados.DataSource = DAOEcommercePrana.ListaEstados("-1", this.Usuario);
                    StoreEstados.DataBind();

                    PagingToolBar1.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtSucursales", null);
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
            this.cBoxClaveCadena.Clear();
            this.txtCadena.Clear();
            this.cBoxClaveSucursal.Clear();
            this.txtSucursal.Clear();
            this.cBoxEstado.Clear();
            this.cBoxActiva.Clear();

            LimpiaGridSucursales();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al Grid de Sucursales
        /// </summary>
        protected void LimpiaGridSucursales()
        {
            this.btnExportExcel.Disabled = true;
            this.btnExportCSV.Disabled = true;

            HttpContext.Current.Session.Add("DtSucursales", null);
            StoreSucursales.RemoveAll();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de sucursales, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelSucursales(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;
            this.btnExportCSV.Disabled = true;

            try
            {
                DataTable dtSucursales = new DataTable();

                dtSucursales = HttpContext.Current.Session["DtSucursales"] as DataTable;

                if (dtSucursales == null)
                {

                    int id_sucursal = String.IsNullOrEmpty(this.cBoxClaveSucursal.SelectedItem.Value) ?
                        -1 : int.Parse(this.cBoxClaveSucursal.SelectedItem.Value);

                    dtSucursales = DAOEcommercePrana.ObtieneReporteSucursales(
                        String.IsNullOrEmpty(this.cBoxClaveCadena.SelectedItem.Value) ?
                        -1 : int.Parse(this.cBoxClaveCadena.SelectedItem.Value),
                        this.txtCadena.Text, id_sucursal, this.txtSucursal.Text,
                        String.IsNullOrEmpty(this.cBoxPais.SelectedItem.Value) ?
                        "" : this.cBoxPais.SelectedItem.Value,
                        String.IsNullOrEmpty(this.cBoxEstado.SelectedItem.Value) ?
                        "" : this.cBoxEstado.SelectedItem.Value,
                        String.IsNullOrEmpty(this.cBoxActiva.SelectedItem.Value) ?
                        -1 : int.Parse(this.cBoxActiva.SelectedItem.Value),
                        Convert.ToDateTime(this.dfFechaInicio.SelectedDate),
                        Convert.ToDateTime(this.dfFechaFin.SelectedDate),
                        this.Usuario);

                    HttpContext.Current.Session.Add("DtSucursales", dtSucursales);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtSucursales.Rows.Count < 1)
                {
                    X.Msg.Alert("Sucursales", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtSucursales.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Sucursales", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepSucPrana.ClicDePaso()",
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
                    int TotalRegistros = dtSucursales.Rows.Count;

                    (this.StoreSucursales.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtSucursales.Clone();
                    DataTable dtToGrid = dtSucursales.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtSucursales.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingToolBar1.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtSucursales.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreSucursales.DataSource = dtToGrid;
                    StoreSucursales.DataBind();

                    this.btnExportExcel.Disabled = false;
                    this.btnExportCSV.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Sucursales", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Sucursales", "Ocurrió un Error al Consultar el Reporte de Sucursales").Show();
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
            LimpiaGridSucursales();

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
        /// Controla el evento onRefresh del grid de sucursales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreSucursales_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelSucursales(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSucPrana")]
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
        [DirectMethod(Namespace = "RepSucPrana")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteSucursales";
                DataTable _dtSucursales = HttpContext.Current.Session["DtSucursales"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtSucursales, reportName);

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
                X.Msg.Alert("Sucursales", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                ws.Cell(rowsCounter, 17).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 19).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 21).SetDataType(XLCellValues.DateTime);
            }

            return ws;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSucPrana")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Pais, estableciendo los Estados correspondientes al país elegido
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceEstados(object sender, EventArgs e)
        {
            try
            {
                StoreEstados.DataSource = DAOEcommercePrana.ListaEstados(this.cBoxPais.SelectedItem.Value, this.Usuario);
                StoreEstados.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Estado", err.Mensaje()).Show();
            }

            catch (Exception )
            {
                X.Msg.Alert("Estado", "Ocurrió un error al establecer los Estados").Show();
            }
        }
    }
}