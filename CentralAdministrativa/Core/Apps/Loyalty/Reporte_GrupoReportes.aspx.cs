using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentroContacto.LogicaNegocio;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace CentroContacto
{
    public partial class Grupo_Reportes : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //StoreCadenaComercial.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaCadenaComercial").Valor, "", -1);
                    //StoreCadenaComercial.DataBind();

                    DataSet dsCadenaComercial = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), 
                        DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaCadenaComercial").Valor, "", -1);

                    List<ColectivaComboPredictivo> cadenaList = new List<ColectivaComboPredictivo>();

                    foreach (DataRow cadena in dsCadenaComercial.Tables[0].Rows)
                    {
                        var cadenaCombo = new ColectivaComboPredictivo()
                        {
                            ID_Colectiva = Convert.ToInt64(cadena["ID_Colectiva"].ToString()),
                            ClaveColectiva = cadena["ClaveColectiva"].ToString(),
                            NombreORazonSocial = cadena["NombreORazonSocial"].ToString()
                        };
                        cadenaList.Add(cadenaCombo);
                    }

                    StoreCadenaComercial.DataSource = cadenaList;
                    StoreCadenaComercial.DataBind();

                    datInicio.SetValue(DateTime.Now);
                    datFinal.SetValue(DateTime.Now);

                    //LlenaEstatus();
                    //LlenaOperadores();
                    //LlenaBeneficiario();
                }
            }
            catch (Exception )
            {
            }
        }

        private string ToXML(DataSet ds)
        {
            using (MemoryStream memori = new MemoryStream())
            {
                using (TextWriter strinWrit = new StreamWriter(memori))
                {
                    XmlSerializer xmlStr = new XmlSerializer(typeof(DataSet));
                    xmlStr.Serialize(strinWrit, ds);
                    return Encoding.UTF8.GetString(memori.ToArray());
                }
            }
        
        }

        private void TipoExportacion()
        {
            if (rbExcel.Checked)
                this.FormatType.Value = "xls";
            else if (rbXml.Checked)
                this.FormatType.Value = "xml";
            else if (rbCsv.Checked)
                this.FormatType.Value = "csv";
        }

        private void BuscaDatosAndBind()
        {
            try
            {

                String tipoColectiva=null;
                DateTime fechaInicio, fechaFin;

                if (datInicio.Value == null || datFinal.Value == null)
                    return;

                fechaInicio = datInicio.SelectedDate;
                fechaFin = datFinal.SelectedDate;

                if (fechaInicio > fechaFin)
                {
                    throw new Exception("La Fecha Inicial es Mayor a la Final.");
                }
                tipoColectiva = cmbCadenaComercial.Value == null ? null : cmbCadenaComercial.Value.ToString();

                var archive = Server.MapPath(@"~\zipFile\archive.zip");
                var temp = Server.MapPath(@"~\temp\");
                if (File.Exists(archive))
                    File.Delete(archive);

                Directory.EnumerateFiles(temp).ToList().ForEach(f => File.Delete(f)); 

                TipoExportacion();
                if (cbClientes.Checked)
                {
                    Store1_Submit(LNReportes.ReporteClientes(fechaInicio, fechaFin, tipoColectiva, "", "", "", this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),true),"Reporte de Clientes",temp);
                }
                if (cbActividades.Checked)
                {
                    Store1_Submit(LNReportes.ReporteActividades(fechaInicio, fechaFin, tipoColectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), true), "Reporte de Actividades", temp);
                }
                if (cbBeneficios.Checked)
                {
                    Store1_Submit(LNReportes.ReporteBeneficios(fechaInicio, fechaFin, tipoColectiva, "", "", this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), true), "Reporte de Beneficios", temp);
                }
                if (cbOperaciones.Checked)
                {
                    Store1_Submit(LNReportes.ReporteOperaciones(fechaInicio, fechaFin, tipoColectiva, "", "", this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), true), "Reporte de Operaciones", temp);
                }
                if (cbLlamadas.Checked)
                {
                    Store1_Submit(LNReportes.ReporteLlamadas(fechaInicio, fechaFin, tipoColectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), true), "Reporte de Llamadas", temp);
                }


                               
                ZipFile.CreateFromDirectory(temp, archive);
                this.Response.ContentType = "application/zip";
                this.Response.AddHeader("Content-Disposition","attachment; filename=Grupo de Reportes.zip");
                this.Response.TransmitFile(archive);
                //this.Response.Flush();
                //this.Response.End();
                
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        protected void Store1_Submit(DataSet data,string nombre,string temp)
        {
            string format = this.FormatType.Value.ToString();
            XmlDataDocument xml = new XmlDataDocument(data);
            StreamWriter FileDocument;
            

            switch (format)
            {
                case "xml":
                    //string strXml = xml.OuterXml;
                    //this.Response.AddHeader("Content-Disposition", "attachment; filename=" + nombre + ".xml");
                    //this.Response.AddHeader("Content-Length", strXml.Length.ToString());
                    //this.Response.ContentType = "application/xml";
                    ////this.Response.Write(strXml);
                    FileDocument = new StreamWriter(temp + nombre + ".xml",false);
                    data.WriteXml(FileDocument);
                    FileDocument.Close();
                    break;

                case "xls":
                    //this.Response.ContentType = "application/vnd.ms-excel";
                    //this.Response.AddHeader("Content-Disposition", "attachment; filename="+nombre+".xls");
                    XslCompiledTransform xtExcel = new XslCompiledTransform();
                    xtExcel.Load(Server.MapPath("xslFiles/Excel.xsl"));
                    //xtExcel.Transform(xml, null, Response.OutputStream);
                    FileDocument = new StreamWriter(temp + nombre + ".xsl", false);
                    xtExcel.Transform(xml, null, FileDocument);
                    FileDocument.Close();
                    //Excel.Application excelApp = new Excel.Application();
                    //Excel.Workbook exWoBo = excelApp.Workbooks.Open(temp+nombre+".xls");
                    //foreach (DataTable table in data.Tables)
                    //{
                    //    Excel.Worksheet excShe = exWoBo.Sheets.Add();
                    //    excShe.Name = nombre;
                    //    for (int i = 1; i < table.Columns.Count + 1; i++)
                    //    {
                    //        excShe.Cells[1, i] = table.Columns[i - 1].ColumnName;
                    //    }
                    //    for (int j = 1; j < table.Columns.Count + 1; j++)
                    //    {
                    //        for (int k = 1; k < table.Columns.Count + 1; k++)
                    //        {
                    //            excShe.Cells[j + 2, k + 1] = table.Rows[j].ItemArray[k].ToString();
                    //        }
                    //    }
                    //    exWoBo.Save();
                    //    exWoBo.Close();
                    //    excelApp.Quit();
                    //}
                    break;

                case "csv":
                    //this.Response.ContentType = "application/octet-stream";
                    //this.Response.AddHeader("Content-Disposition", "attachment; filename=" + nombre + ".csv");
                    XslCompiledTransform xtCsv = new XslCompiledTransform();
                    xtCsv.Load(Server.MapPath("xslFiles/Csv.xsl"));
                    //xtCsv.Transform(xml, null, Response.OutputStream);
                    FileDocument = new StreamWriter(temp + nombre + ".csv", false);
                    xtCsv.Transform(xml, null, FileDocument);
                    FileDocument.Close();
                    //StringBuilder sb = new StringBuilder();
                    //IEnumerable<string> columnNames = data.Tables[0].Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                    //sb.AppendLine(string.Join(",",columnNames));
                    //foreach(DataRow row in data.Tables[0].Rows){
                    //    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    //    sb.AppendLine(string.Join(",", fields));
                    //}
                    //File.WriteAllText(temp + nombre + ".csv",sb.ToString());
                    break;
            }            
            
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                BuscaDatosAndBind();

            }
            catch (Exception err)
            {
                //Store1.RemoveAll();
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                //GridPanel1.GetStore().RemoveAll();
                FormPanel1.Reset();

            }
            catch (Exception err)
            {
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

        }
    }
}