using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALComercioElectronico.BaseDatos;
using Ext.Net;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;

namespace ComercioElectronico
{
    public partial class VerProductosFiltro : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Ext.Net.Theme"] = Ext.Net.Theme.Default;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());
            var user = this.Usuario;


            var result = DaoProductosCombos.GetFamilias(user, idApp);


            //ComboFamilia.Data = result;
            //ComboFamilia.DataBind();

            ComboFamilia.GetStore().Data = result;
            ComboFamilia.GetStore().DataBind();


        }

        protected void VerProductosFiltrados(object sender, DirectEventArgs e)
        {
            var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            int familia = 0;
            string sku = null;
            string nombre = null;

            //if(Familia.Value!=null)familia=Familia.Value.ToString();

            if (Sku.Value != null)
                sku = Sku.Value.ToString();

            if (Nombre.Value != null)
                nombre = Nombre.Value.ToString();

            if (ComboFamilia.SelectedItem.Value != null)
                familia = Convert.ToInt32(ComboFamilia.SelectedItem.Value);
        
            DataSet dsData = DaoProductosCombos.GetProductosLayout(user, idApp, familia, sku, nombre);

            if (dsData.Tables[0].Rows.Count == 0)
            {
                X.Msg.Alert("Búsqueda de Productos", "No existen coincidencias con los datos solicitados").Show();
            }
            else
            {
                gridPanelBase.GetStore().DataSource = dsData;
                gridPanelBase.GetStore().DataBind();
            }

            //return values;
        }

        protected void Store1_Submit(object sender, StoreSubmitDataEventArgs e)
        {
            string format = this.FormatType.Value.ToString();

            XmlNode xml = e.Xml;

            this.Response.Clear();

            switch (format)
            {
                case "xml":
                    string strXml = xml.OuterXml;
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.xml");
                    this.Response.AddHeader("Content-Length", strXml.Length.ToString());
                    this.Response.ContentType = "application/xml";
                    this.Response.Write(strXml);
                    break;

                case "xls":
                    this.Response.ContentType = "application/vnd.ms-excel";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.xls");
                    XslCompiledTransform xtExcel = new XslCompiledTransform();
                    xtExcel.Load(Server.MapPath("xslFiles/Excel.xsl"));
                    xtExcel.Transform(xml, null, Response.OutputStream);

                    break;

                case "csv":
                    this.Response.ContentType = "application/octet-stream";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.csv");
                    XslCompiledTransform xtCsv = new XslCompiledTransform();
                    xtCsv.Load(Server.MapPath("xslFiles/Csv.xsl"));
                    xtCsv.Transform(xml, null, Response.OutputStream);

                    break;
            }

            this.Response.End();
        }

        /// <summary>
        /// Exporta el grid de resultados al backend, para generar y exportarlo a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            string gridResultJson = e.ExtraParams["GridToExport"];
            string reportName = "Productos";

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
                ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.Boolean);
                ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 9).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 13).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 14).SetDataType(XLCellValues.Boolean);
                ws.Cell(rowsCounter, 15).SetDataType(XLCellValues.Boolean);
                ws.Cell(rowsCounter, 16).SetDataType(XLCellValues.Boolean);
            }

            return ws;
        }
    }
}