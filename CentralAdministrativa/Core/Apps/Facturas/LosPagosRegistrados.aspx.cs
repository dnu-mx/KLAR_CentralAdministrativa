using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Xsl;

namespace Facturas
{
    public partial class LosPagosRegistrados : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    datPagoFin.SetValue(DateTime.Now);
                    datPagoInicio.SetValue(DateTime.Now);
                }

            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }


        protected void btnBuscar_Click(object sender, DirectEventArgs e)
        {
            try
            {
                BindDataPagos(datPagoInicio.SelectedDate, datPagoFin.SelectedDate, txtCadenaComercial.Text);
            }
            catch (Exception err)
            {
                Loguear.Error(err, "AsignacionDePagos");

            }
        }

        private void BindDataPagos(DateTime FechaInicio, DateTime FechaFin, string cadenaComrercial)
        {
            DataSet dsPagos = DAOPoliza.ObtienePagosXCadenaComercial(FechaInicio, FechaFin, cadenaComrercial, this.Usuario.UsuarioTemp, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

            if (dsPagos.Tables[0].Rows.Count == 0)
            {
                X.Msg.Alert("Búsqueda de Pagos", "No existen coincidencias con los datos solicitados").Show();
            }
            else
            {
                GridPanel2.GetStore().DataSource = dsPagos;
                GridPanel2.GetStore().DataBind();
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

        [DirectMethod(Namespace = "Facturas")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            try
            {
                Int64 ID_Poliza = Int64.Parse(e.ExtraParams["ID_Poliza"]);
                hdnIdPoliza.Text = e.ExtraParams["ID_Poliza"].ToString();

                Int64 ID_Factura = Int64.Parse(e.ExtraParams["ID_Factura"]);
                hdnIdFactura.Text = e.ExtraParams["ID_Factura"].ToString();

                String EjecutarComando = (String)e.ExtraParams["Comando"];                

                String eMailEmisor = (String)(e.ExtraParams["EmailEmisor"]);
                String eMailReceptor = (String)(e.ExtraParams["EmailReceptor"]);


                switch (EjecutarComando)
                {
                    case "DescargarPDF":
                        Factura LaFactura = DAOFactura.ObtieneFacturaPago(ID_Factura, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                        this.DescargarArchivoPDF(LaFactura);

                        return;

                    case "CFDI":
                        txtCorreos.Text = eMailReceptor + ";" + eMailEmisor;

                        frmSendMail.Visible = true;
                        frmSendMail.Hidden = false;

                        break;

                    case "VerFacturas":
                        storeFactura.DataSource = DAOPoliza.ObtieneFacturaPagos(ID_Poliza);
                        storeFactura.DataBind();

                        winFaturas.Visible = true;
                        winFaturas.Hidden = false;

                        return;
                }

                BuscaDatosAndBind(ID_Poliza);

                X.Msg.Notify("Pagos", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();
            }
            catch (Interfases.Exceptiones.CAppException err)
            {
                X.Msg.Alert("Pagos", err.Mensaje()).Show();
                
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                
                X.Msg.Notify("Pagos", err.Message).Show();
                X.Msg.Notify("Pagos", "Comando ejecutado con <br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
            }

        }

        private void BuscaDatosAndBind( Int64 ID_Poliza)
        {
            try
            {
                GridPanel3.GetStore().DataSource = DAOPoliza.ObtieneFacturaPagos(ID_Poliza);
                GridPanel3.GetStore().DataBind();

            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                throw err;
            }
        }

        protected void GenerarArchivos(Factura lafactura)
        {
            try
            {
                DataTable Dt = new DataTable();//

                CrystalDecisions.CrystalReports.Engine.ReportDocument report = new CrystalDecisions.CrystalReports.Engine.ReportDocument();

                String Path2 = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLosRPT").Valor;
                String unPath = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor;

                //genera el QR
                String elCodigoQR = "?re=" + lafactura.Emisora.RFC + "&rr=" + lafactura.Receptora.RFC + "&tt=" + lafactura.ImporteTotal.ToString("N6").Replace(",", "") + "&id=" + lafactura.UUID;
                String NombreQR = unPath + "QR_" + lafactura.Serie + lafactura.Folio + ".png";
                var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                var qrCode = qrEncoder.Encode(elCodigoQR);


                var renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                using (var stream = new FileStream(NombreQR, FileMode.Create))
                    renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);

                //Genera el XML
                try
                {
                    if (lafactura.TipoComprobante.ToUpper().Equals("INGRESO"))
                    {
                        String unPath2 = Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor;
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(lafactura.XMLCFDI);
                        xDoc.Save(unPath2 + "Factura_" + lafactura.Serie + lafactura.Folio + ".xml");
                    }
                }
                catch (Exception)
                {
                }


                //genera el PDF
                report.Load(Path2 + lafactura.UrlReporte);
                Dt = DAOFactura.ObtieneDataSetFactura(lafactura.ID_Factura).Tables[0];
                report.SetDataSource(Dt);
                String NombreArchivo = "Factura_" + lafactura.Serie + lafactura.Folio + ".pdf";
                report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, unPath + NombreArchivo);

                report.Close();
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                throw err;
            }
            finally
            {

            }
        }

        protected void DescargarArchivoPDF(Factura laFactura)
        {

            try
            {
                GenerarArchivos(laFactura);

                String NombreArchivo = "";

                NombreArchivo = "Factura_" + laFactura.Serie + laFactura.Folio + ".pdf";

                String PathFiles = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor;

                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.ContentType = "Application/PDF";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + NombreArchivo);
                Response.TransmitFile(PathFiles + NombreArchivo);
                Response.Flush();
                Response.End();

            }

            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                DALAutorizador.Utilidades.Loguear.Error(err, "");
            }
        }

        protected void EnviarCorreo(object sender, DirectEventArgs e)
        {
            try
            {
                Int64 ID_Factura = Int64.Parse(hdnIdFactura.Text);

                Factura LaFacturaToSend = DAOFactura.ObtieneFacturaPago(ID_Factura, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                String laFacturaPDF;

                laFacturaPDF = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor + "Factura_" + LaFacturaToSend.Serie + LaFacturaToSend.Folio + ".pdf";

                String LaFacturaXML = "";

                LaFacturaXML = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor + "Factura_" + LaFacturaToSend.Serie + LaFacturaToSend.Folio + ".xml";

                String elHtml = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "Mailhtml").Valor + "MailParaEnvioFacturas.html";

                StringBuilder ElCuerpodelMail = new StringBuilder(File.ReadAllText(elHtml));

                //Genera los Archivos
                GenerarArchivos(LaFacturaToSend);

                //Atache los archivos
                Attachment uno, dos;
                uno = new Attachment(laFacturaPDF);

                try
                {
                    dos = new Attachment(LaFacturaXML);
                }
                catch (Exception err)
                {
                    Loguear.Error(err, this.Usuario.ClaveUsuario);
                    dos = null;
                }

                Emailing.Send(txtCorreos.Text, ElCuerpodelMail.ToString(), "Factura Emitida por " + LaFacturaToSend.Emisora.NombreORazonSocial, uno, dos);

                if (uno != null)
                    uno.Dispose();

                if (dos != null)
                    dos.Dispose();

                uno = null;
                dos = null;

                X.Js.AddScript("#{frmSendMail}.setVisible(false);");

                Loguear.EntradaSalida("Se envio la Factura con folio [" + LaFacturaToSend.Folio + "] a los Correos: " + txtCorreos.Text, this.Usuario.ClaveUsuario, false);

                X.Msg.Notify("Envio de Email", "Factura enviada <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                BuscaDatosAndBind(Int64.Parse(hdnIdPoliza.Text));
            }

            catch (Exception err)
            {
                X.Msg.Notify("Envio de Email", err.Message).Show();
                X.Msg.Notify("Envio de Email", "<br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }
    }
}