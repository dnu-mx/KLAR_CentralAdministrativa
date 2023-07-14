using ClosedXML.Excel;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using DALPuntoVentaWeb.LogicaNegocio;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;

namespace TpvWeb
{
    public partial class Reporte_PromocionesLoyalty : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Reporte Promociones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    datInicio.SetValue(DateTime.Now);
                    datFinal.SetValue(DateTime.Now);

                    string claveCadena = Configuracion.Get(Guid.Parse
                        (ConfigurationManager.AppSettings["IDApplication"].ToString()), 
                        "ClaveTipoColectivaCadenaComercial").Valor;

                    DataSet dsCadenas = DAOReportes.ObtieneColectivasFiltrosPromociones(-1, claveCadena,
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

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
                }
            }

            catch (Exception err)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }
        
        /// <summary>
        /// Controla el evento de selección de un ítem del combo de cadenas comerciales, llenando
        /// el combo de sucursales con las correspondientes de base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void LlenaSucursales(object sender, EventArgs e)
        {
            try
            {
                StoreSucursal.RemoveAll();
                cmbSucursal.Reset();
                cmbOperador.Reset();

                string claveSucursal = Configuracion.Get(Guid.Parse
                    (ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "ClaveTipoColectivaSucursal").Valor;

                StoreSucursal.DataSource = DAOReportes.ObtieneColectivasFiltrosPromociones(
                    int.Parse(this.cmbCadena.SelectedItem.Value), claveSucursal, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreSucursal.DataBind();
            }

            catch (Exception err)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Reporte de Promociones", "Ocurrió un Error al Obtener la Lista de Sucursales").Show();
            }
        }

        /// <summary>
        /// Controla el evento de selección de un ítem del combo de sucursales, llenando
        /// el combo de operadores con los correspondientes de base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void LlenaOperadores(object sender, EventArgs e)
        {
            try
            {
                StoreOperador.RemoveAll();
                cmbOperador.Reset();

                string claveOperador = Configuracion.Get(Guid.Parse
                    (ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "ClaveTipoColectivaOperador").Valor;

                StoreOperador.DataSource = DAOReportes.ObtieneColectivasFiltrosPromociones(
                    int.Parse(this.cmbCadena.SelectedItem.Value), claveOperador, 
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreOperador.DataBind();
            }

            catch (Exception err)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Reporte de Promociones", "Ocurrió un Error al Obtener la Lista de Operadores").Show();
            }
        }

        /// <summary>
        /// Consulta el reporte a base de datos, validando los datos del filtro y ligando la
        /// información obtenida al Grid correspondiente
        /// </summary>
        private void ConsultaReporteYLigaGrid()
        {
            DateTime FechaInicial, FechaFinal;

            if (datInicio.Value == null || datFinal.Value == null)
                return;


            FechaInicial = datInicio.SelectedDate;
            FechaFinal = datFinal.SelectedDate;

            if (FechaInicial > FechaFinal)
            {
                throw new Exception("La fecha inicial debe ser menor o igual a la fecha final");
            }

            DataSet dsReporte = LNReportes.ListarPromocionesLoyalty(FechaInicial, FechaFinal,
                String.IsNullOrEmpty(this.cmbCadena.SelectedItem.Value) ? -1 : int.Parse(this.cmbCadena.SelectedItem.Value),
                String.IsNullOrEmpty(this.cmbSucursal.SelectedItem.Value) ? "" : this.cmbSucursal.SelectedItem.Value,
                String.IsNullOrEmpty(this.cmbOperador.SelectedItem.Value) ? "" : this.cmbOperador.SelectedItem.Value,
                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

            if (dsReporte == null)
            {
                X.Msg.Alert("Reporte de Promociones", "No Existen Promociones con los Filtros Indicados.").Show();
            }
            else
            {
                GridPanelReporte.GetStore().DataSource = dsReporte;
                GridPanelReporte.GetStore().DataBind();
            }            
        }

        /// <summary>
        /// Controla el evento SUBMIT al querer exportar el reporte a alguno de los formatos seleccionados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void StoreReporte_Submit(object sender, StoreSubmitDataEventArgs e)
        {
            string format = this.FormatType.Value.ToString();

            XmlNode xml = e.Xml;

            this.Response.Clear();

            switch (format)
            {
                case "xml":
                    string strXml = xml.OuterXml;
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.xml");
                    this.Response.AddHeader("Content-Length", strXml.Length.ToString());
                    this.Response.ContentType = "application/xml";
                    this.Response.Write(strXml);
                    break;

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
        /// Controla el evento Click al botón Buscar del formulario de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                StoreReporte.RemoveAll();
                ConsultaReporteYLigaGrid();
            }

            catch (Exception ex)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Promociones", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            StoreReporte.RemoveAll();
            FormPanelFiltros.Reset();
        }

        /// <summary>
        /// Exporta el grid de resultados al backend, para generar y exportarlo a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            string gridResultJson = e.ExtraParams["GridToExport"];
            string reportName = "Promociones";

            XmlNode gridResultXml = JSON.DeserializeXmlNode("{records:{record:" + gridResultJson + "}}");
            XmlTextReader xtr = new XmlTextReader(gridResultXml.OuterXml, XmlNodeType.Element, null);

            DataSet ds = new DataSet();
            ds.ReadXml(xtr);

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(reportName);

            //Se inserta la tabla completa a la hoja de Excel
            ws.Cell(1, 1).InsertTable(ds.Tables[0].AsEnumerable());

            //Se da el formato deseado a las columnas
            ws = FormatWsColumns(ws, ws.Column(1).CellsUsed().Count());

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
        protected IXLWorksheet FormatWsColumns(IXLWorksheet ws, int rowsNum)
        {
            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
            }

            return ws;
        }
    }
}