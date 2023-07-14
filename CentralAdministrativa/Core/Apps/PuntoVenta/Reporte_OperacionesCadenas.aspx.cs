using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.LogicaNegocio;
using DALCentralAplicaciones;
using Ext.Net;
using Log_PCI.Entidades;
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
    public partial class Reporte_OperacionesCadenas : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DataSet dsCadenas = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaCadenaComercial").Valor, "", -1);

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
                    
                    StoreCadenaComercial.DataSource = ListaCadenas;
                    StoreCadenaComercial.DataBind();

                    this.datInicio.SetValue(DateTime.Now);
                    this.datInicio.MaxDate = DateTime.Today;

                    this.datFinal.SetValue(DateTime.Now);
                    this.datFinal.MaxDate = DateTime.Today;

                    LlenaEstatus();
                    LlenaOperadores();
                    LlenaBeneficiario();
                }
            }
            catch (Exception )
            {
            }
        }

        private void BuscaDatosAndBind()
        {
            try
            {

                String Afiliacion, Sucursal, Terminal, Operador, Estatus, Telefono, Benefiario;
                int ID_CadenaComercial;
                DateTime fechaInicial, FechaFinal;


                if (datInicio.Value == null || datFinal.Value == null)
                    return;


                fechaInicial = datInicio.SelectedDate;
                FechaFinal = datFinal.SelectedDate;

                if (fechaInicial > FechaFinal)
                {
                    throw new Exception("La Fecha Inicial es Mayor a la Final.");
                }


                ID_CadenaComercial = cmbCadenaComercial.Value == null ? -1 : int.Parse(cmbCadenaComercial.Value.ToString());
                Afiliacion = cmbAfiliacion.Value == null ? "-1" : cmbAfiliacion.Value.ToString();
                Sucursal = cmbSucursal.Value == null ? "-1" : cmbSucursal.Value.ToString();
                Terminal = cmbTerminal.Value == null ? "-1" : cmbTerminal.Value.ToString();
                Operador = cmbOperador.Value == null ? "-1" : cmbOperador.Value.ToString();
                Estatus = cmbEstatus.Value == null ? "-1" : cmbEstatus.Value.ToString();
                Benefiario = cmbBeneficiario.Value == null ? "-1" : cmbBeneficiario.Value.ToString();
                Telefono = txtTelefono.Text.Trim().Length == 0 ? "-1" : txtTelefono.Text.Trim();

                GridPanel1.GetStore().DataSource = LNOperaciones.ListarOperacionesCadena(fechaInicial, FechaFinal, Sucursal, Afiliacion, Terminal, Operador, Benefiario, Estatus, Telefono, ID_CadenaComercial, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                GridPanel1.GetStore().DataBind();

            }
            catch (Exception err)
            {
                throw err;
            }
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



        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {

                BuscaDatosAndBind();

            }
            catch (Exception err)
            {
                Store1.RemoveAll();
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                GridPanel1.GetStore().RemoveAll();
                FormPanel1.Reset();

            }
            catch (Exception err)
            {
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

        }
        protected void LlenaAfiliaciones(object sender, EventArgs e)
        {
            try
            {
                StoreAfiliacion.RemoveAll();
                cmbAfiliacion.Reset();
                cmbTerminal.Reset();
                cmbOperador.Reset();

                cmbBeneficiario.Reset();
                StoreAfiliacion.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaAfiliacion").Valor, cmbSucursal.Value.ToString(), Int64.Parse(cmbCadenaComercial.Value.ToString()));
                StoreAfiliacion.DataBind();

            }
            catch (Exception err)
            {
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

        }

        protected void LlenaEstatus()
        {
            try
            {
                StoreEstatus.RemoveAll();
                StoreEstatus.DataSource = DAOCatalogos.ListaEstatusOperacion(new LogHeader());//(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaAfiliacion").Valor, cmbSucursal.Value.ToString());
                StoreEstatus.DataBind();

                cmbEstatus.SetValue("1");

            }
            catch (Exception err)
            {
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

        }

        protected void LlenaBeneficiario()
        {
            try
            {

                StoreBeneficiario.RemoveAll();
                //cmbTerminal.Reset();
                //cmbOperador.Reset();
                StoreBeneficiario.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaBeneficiario").Valor, "0", -1);
                StoreBeneficiario.DataBind();


            }
            catch (Exception err)
            {
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

        }

        protected void LlenaSucursales(object sender, EventArgs e)
        {
            try
            {

                StoreSucursal.RemoveAll();
                //cmbTerminal.Reset();
                //cmbOperador.Reset();
                StoreSucursal.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaSucursal").Valor, "0", Int64.Parse(cmbCadenaComercial.Value.ToString()));
                StoreSucursal.DataBind();


            }
            catch (Exception err)
            {
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

        }
        protected void LlenaTerminales(object sender, EventArgs e)
        {
            try
            {
                StoreTerminal.RemoveAll();
                cmbTerminal.Reset();
                cmbOperador.Reset();
                cmbBeneficiario.Reset();
                StoreTerminal.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaTerminal").Valor, cmbAfiliacion.Value.ToString(), Int64.Parse(cmbCadenaComercial.Value.ToString()));
                StoreTerminal.DataBind();

            }
            catch (Exception err)
            {
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

        }

        protected void LlenaOperadores()
        {
            try
            {
                StoreOperador.RemoveAll();
                cmbOperador.Reset();

                //StoreOperador.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaOperador").Valor, cmbTerminal.Value.ToString());
                //StoreOperador.DataBind();

                try
                {
                    StoreOperador.DataSource = DAOCatalogos.ListaTiposColectivasPorTipoColectiva(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaOperador").Valor);
                    StoreOperador.DataBind();
                }
                catch (Exception )
                {
                }

            }
            catch (Exception err)
            {
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }
        }

        /// <summary>
        /// Exporta el grid de resultados al backend, para generar y exportarlo a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            string gridResultJson = e.ExtraParams["GridToExport"];
            string reportName = "VentasCadena";

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
                ws.Cell(rowsCounter, 15).SetDataType(XLCellValues.Number);
            }

            return ws;
        }
    }
}