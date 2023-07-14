using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Xml;
using System.Xml.Xsl;

namespace CentroContacto
{
    public partial class Reporte_LlamadasTiendasDiconsa : PaginaBaseCAPP
    {
        /// <summary>
        /// Controla el evento de carga inicial de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //Prestablecemos el periodo de consulta a 7 días
                    datInicio.SetValue(DateTime.Today.AddDays(-7));
                    datInicio.MaxDate = DateTime.Today;

                    datFinal.SetValue(DateTime.Today);
                    datFinal.MaxDate = DateTime.Today;
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }


        /// <summary>
        /// Controla el evento Click al botón Buscar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                this.StoreResultados.DataSource =
                   LNReportes.ReporteLlamadasTiendasDiconsa(Convert.ToDateTime(datInicio.Value), 
                        Convert.ToDateTime(datFinal.Value), this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                this.StoreResultados.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Reporte de Llamadas", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Reporte de Llamadas", "Ocurrió un Error al Consultar los Datos del Reporte").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            GridResultados.GetStore().RemoveAll();
            FormPanelBusqueda.Reset();
        }

        /// <summary>
        /// Controla el evento SUBMIT al exportar el reporte en XLS, CSV o XML
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento submit del store</param>
        protected void StoreResultados_Submit(object sender, StoreSubmitDataEventArgs e)
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
    }
}