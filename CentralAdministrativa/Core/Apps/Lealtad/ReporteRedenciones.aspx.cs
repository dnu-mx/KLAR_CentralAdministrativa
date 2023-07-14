using ClosedXML.Excel;
using DALLealtad.BaseDatos;
using DALLealtad.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;

namespace Lealtad
{
    public partial class ReporteRedenciones : DALCentralAplicaciones.PaginaBaseCAPP
    {
        public class CadenaComboPredictivo
        {
            public Int64 ID_Colectiva { get; set; }
            public String ClaveColectiva     { get; set; }
            public String NombreORazonSocial { get; set; }
        }

        public class SucursalComboPredictivo
        {
            public Int64 ID_Colectiva { get; set; }
            public String ClaveColectiva { get; set; }
            public String NombreORazonSocial { get; set; }
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
                    PrecargaFiltros();
                }

                if (!X.IsAjaxRequest)
                {
                    ;
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Realiza la precarga de catálogos de los controles en el panel de filtros
        /// </summary>
        protected void PrecargaFiltros()
        {
            StoreEmisor.DataSource = DAOPromociones.ListaEmisores(this.Usuario,
                       Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            StoreEmisor.DataBind();

            DataSet dsCadenas = DAOPromociones.ListaCadenasComerciales(this.Usuario,
                       Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

            List<CadenaComboPredictivo> ComboList = new List<CadenaComboPredictivo>();

            foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
            {
                var cadenaCombo = new CadenaComboPredictivo()
                {
                    ID_Colectiva = Convert.ToInt32(cadena["ID_Colectiva"].ToString()),
                    ClaveColectiva = cadena["ClaveColectiva"].ToString(),
                    NombreORazonSocial = cadena["NombreORazonSocial"].ToString()
                };
                ComboList.Add(cadenaCombo);
            }

            StoreCadena.DataSource = ComboList;
            StoreCadena.DataBind();

            StoreEstados.DataSource = DAOEcommerceSams.ListaEstados(this.Usuario);
            StoreEstados.DataBind();

            dfFechaInicio.SetValue(DateTime.Today);

            dfFechaFin.MaxDate = DateTime.Today;
            dfFechaFin.SetValue(DateTime.Today);
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Cadena, estableciendo las
        /// sucursales que le corresponden
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void ObtieneSucursales(object sender, EventArgs e)
        {
            try
            {
                DataSet dsSucursales = DAOPromociones.ListaSucursalesDeCadena(
                    String.IsNullOrEmpty(this.cBoxCadena.SelectedItem.Value) ? -1 :
                    Convert.ToInt64(this.cBoxCadena.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                List<SucursalComboPredictivo> sucursalesList = new List<SucursalComboPredictivo>();

                foreach (DataRow sucursal in dsSucursales.Tables[0].Rows)
                {
                    var sucursalCombo = new SucursalComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt32(sucursal["ID_Colectiva"].ToString()),
                        ClaveColectiva = sucursal["ClaveColectiva"].ToString(),
                        NombreORazonSocial = sucursal["NombreORazonSocial"].ToString()
                    };
                    sucursalesList.Add(sucursalCombo);
                }

                StoreSucursal.DataSource = sucursalesList;
                StoreSucursal.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Sucursales", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Sucursales", "Ocurrió un Error al Consultar las Sucursales de la Cadena").Show();
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Estado del panel de filtros,
        /// obteniendo la lista de ciudades que corresponden al estado seleccionado
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void BuscaCiudades(object sender, EventArgs e)
        {
            try
            {
                StoreCiudades.DataSource = DAOEcommerceSams.ListaCiudades(
                        this.cBoxEstado.SelectedItem.Value, this.Usuario);
                StoreCiudades.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Ciudades", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Ciudades", "Ocurrió un Error al Buscar las Ciudades").Show();
            }
        }


        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            this.cBoxEmisor.Clear();
            this.cBoxCadena.Clear();
            this.cBoxSucursal.Clear();
            this.cBoxEstado.Clear();
            this.cBoxCiudad.Clear();
            this.dfFechaInicio.SetValue(DateTime.Today);
            this.dfFechaFin.SetValue(DateTime.Today);

            this.btnExportExcel.Disabled = true;
            this.btnExportCSV.Disabled = true;
            StoreRedenciones.RemoveAll();
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
                DataSet dsRedenciones = DAOPromociones.ObtieneReporteRedenciones(
                    String.IsNullOrEmpty(this.cBoxEmisor.SelectedItem.Value) ? "" :
                        this.cBoxEmisor.SelectedItem.Value,
                    String.IsNullOrEmpty(this.cBoxCadena.SelectedItem.Value) ? -1 :
                        Convert.ToInt64(this.cBoxCadena.SelectedItem.Value), 
                    String.IsNullOrEmpty(this.cBoxSucursal.SelectedItem.Value) ? -1 :
                        Convert.ToInt64(this.cBoxSucursal.SelectedItem.Value),
                    String.IsNullOrEmpty(this.cBoxEstado.SelectedItem.Value) ? "" :
                        this.cBoxEstado.SelectedItem.Value, 
                    String.IsNullOrEmpty(this.cBoxCiudad.SelectedItem.Value) ? "" :
                        this.cBoxCiudad.SelectedItem.Value,
                    Convert.ToDateTime(this.dfFechaInicio.SelectedDate),
                    Convert.ToDateTime(this.dfFechaFin.SelectedDate), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (dsRedenciones.Tables[0].Rows.Count < 1)
                {
                    X.Msg.Alert("Redenciones", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else
                {
                    StoreRedenciones.DataSource = dsRedenciones;
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
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Redenciones", "Ocurrió un Error al Consultar el Reporte de Redenciones").Show();
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
            string reportName = "ReporteRedenciones";

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
        protected IXLWorksheet FormatWsColumns(IXLWorksheet ws, int rowsNum)
        {
            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, 9).SetDataType(XLCellValues.DateTime);
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
    }
}