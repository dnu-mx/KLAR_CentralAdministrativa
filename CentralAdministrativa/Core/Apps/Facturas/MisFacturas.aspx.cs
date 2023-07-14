using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.LogicaNegocio;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Facturas.LogicaNegocio;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace Facturas
{
    public partial class MisFacturas : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    datInicio.SetValue(DateTime.Now);
                    datFinal.SetValue(DateTime.Now);

                    this.LlenaEmisores();
                    this.LlenaReceptores();
                    this.LlenaEstatus();
                }

            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        protected void EjecutarComandoDetallePago(object sender, DirectEventArgs e)
        {
            try
            {
                Int64 ID_PagoAsignado = Int64.Parse(e.ExtraParams["ID"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                Int64 ID_Factura = (Int64)HttpContext.Current.Session["ID_Factura"];

                switch (EjecutarComando)
                {

                    case "DeletePago":
                        //eNVIAR POR EMAIL

                        if (LNFactura.DesasignarPagoAFactura(ID_PagoAsignado))
                        {
                            X.Msg.Notify("Pagos", "Eliminación <br />  <br /> <b> A U T O R I Z A D A </b> <br />  <br /> ").Show();
                        }
                        else
                        {
                            X.Msg.Notify("Pagos", "> <b>Eliminación <br />  <br /> <b> D E C L I N A D A </br />  <br /> ").Show();
                        }

                        BuscaDatosAndBind();

                        storePagos.DataSource = DAOFactura.ObtienePagosFactura(ID_Factura);
                        storePagos.DataBind();

                        return;
                }

               

               
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            try
            {
                X.Mask.Show(new MaskConfig { Msg = "Procesando..." });

                Int64 ID_Factura = Int64.Parse(e.ExtraParams["ID"]);
                String elEmailColectiva = (String)(e.ExtraParams["EmailReceptor"]);
                String elEmailColectivaEmisora = (String)(e.ExtraParams["EmailEmisor"]);
                Int64 ID_Receptor = Int64.Parse(e.ExtraParams["ID_Receptor"]); 
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                HttpContext.Current.Session.Add("ID_Factura", ID_Factura);
                HttpContext.Current.Session.Add("ID_Receptor", ID_Receptor);

                switch (EjecutarComando)
                {
                    case "Foliar":
                        bool Resp = LNFactura.AsignaFolioFactura(ID_Factura);

                        if (!Resp)
                        {
                            throw new Exception("NO SE PUDO ASIGNAR FOLIO A LA FACTURA");
                        }

                        Loguear.Evento("ID_Factura " + ID_Factura + " Foliada.", this.Usuario.ClaveUsuario);
                        break;

                    case "Timbrar":
                        Factura LaFactura = DAOFactura.ObtieneFactura(ID_Factura, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                        int resp = -1;
                      
                        if (LaFactura.TipoComprobante.ToUpper().Equals("INGRESO") || LaFactura.TipoComprobante.ToUpper().Equals("I"))
                        {
                            CFDI XmlFactura = new CFDI(Configuracion.Get(
                                Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                                "FileXML_CFDI3.3").Valor, LaFactura);

                            resp = LNTimbrados.TimbrarFactura(LaFactura, XmlFactura.ToString(), this.Usuario);
                        }
                        else if(LaFactura.TipoComprobante.ToUpper().Equals("PAGO") || LaFactura.TipoComprobante.ToUpper().Equals("P"))
                        {
                            Pago unPago = DAOFactura.ObtienePolizaEImporteFacturaPago(LaFactura.ID_Factura);
                            bool bResp = LNPagos.TimbrarPago(unPago.ID_Poliza,unPago.ImportePAgado, this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                            resp = bResp ? 0 : -1;
                        }
                        else
                        {
                            CFDI XmlFactura = new CFDI(Configuracion.Get(
                                Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                                "FileXML_CFDI3.3").Valor, LaFactura);
                            resp = LNTimbrados.TimbrarFactura(LaFactura, XmlFactura.ToString(), this.Usuario);
                        }

                        if (resp == 0)
                        {
                            if (LaFactura.TipoComprobante.ToUpper().Equals("INGRESO") || LaFactura.TipoComprobante.ToUpper().Equals("I"))
                            {
                                LNFactura.CambiaEstatusFactura(ID_Factura, 3);//Cambia el estatus a Timbrado

                                Loguear.Evento("ID_Factura " + ID_Factura + " Timbrada.", this.Usuario.ClaveUsuario);
                            }
                            else if (LaFactura.TipoComprobante.ToUpper().Equals("PAGO") || LaFactura.TipoComprobante.ToUpper().Equals("P"))
                            {
                                LNFactura.CambiaEstatusFactura(ID_Factura, 3);//Cambia el estatus a Timbrado

                                Loguear.Evento("ID_Factura " + ID_Factura + " Timbrada.", this.Usuario.ClaveUsuario);
                            }
                            else
                            {
                                LNFactura.CambiaEstatusFactura(ID_Factura, 5);//Cambia el estatus a Confirmada

                                Loguear.Evento("ID_Factura " + ID_Factura + " Confirmada.", this.Usuario.ClaveUsuario);
                            }
                        }
                        else
                        {
                        }

                        break;

                    case "Reenviar":
                        //eNVIAR POR EMAIL
                        //Asginar el email del Receptor como envio principal.
                        txtCorreos.Text = elEmailColectiva + ";" +elEmailColectivaEmisora;//LaFacturaToSend.Receptora.Email;

                        X.Mask.Hide();
                        frmSendMail.Visible = true;
                        frmSendMail.Hidden = false;

                        return;
                    //break; 

                    case "Detalles":
                        //eNVIAR POR EMAIL
                        StoreDetalleFactura.DataSource = DAOFactura.ObtieneDetallesFactura(ID_Factura);
                        StoreDetalleFactura.DataBind();
                        //Asginar el email del Receptor como envio principal.
                        txtCorreos.Text = elEmailColectiva;//LaFacturaToSend.Receptora.Email;

                        X.Mask.Hide();
                        winDetallesFactura.Visible = true;
                        winDetallesFactura.Hidden = false;

                        return;

                    case "DescargarPDF":
                        Factura LaFactura2 = DAOFactura.ObtieneFactura(ID_Factura, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                        this.DescargarArchivoPDF(LaFactura2);
                        X.Mask.Hide();

                        return;

                    case "Graficar":
                        X.Js.AddScript("#{Panel4}.reload();");

                        X.Mask.Hide();
                        frmGrafica.Visible = true;
                        frmGrafica.Hidden = false;

                        return;

                    case "Cancelar":
                        bool Resp2 = LNFactura.CancelaFactura(ID_Factura);

                        if (!Resp2)
                        {
                            throw new Exception("NO SE PUDO CANCELAR LA FACTURA");
                        }
                        Loguear.Evento("ID_Factura " + ID_Factura + " Cancelada.", this.Usuario.ClaveUsuario);
                        break;

                    case "Eliminar":
                        bool Resp3 = LNFactura.EliminarSugerenciaDeFactura(ID_Factura);

                        if (!Resp3)
                        {
                            throw new Exception("NO SE PUDO CANCELAR LA FACTURA");
                        }

                        Loguear.Evento("ID_Factura " + ID_Factura + " Eliminada.", this.Usuario.ClaveUsuario);
                        break;

                    case "Pagos":
                        //eNVIAR POR EMAIL

                        storePagos.DataSource = DAOFactura.ObtienePagosFactura(ID_Factura);
                        storePagos.DataBind();

                        X.Mask.Hide();
                        winPagos.Visible = true;
                        winPagos.Hidden = false;
                        
                        return;
                }

                X.Mask.Hide();
                BuscaDatosAndBind();

                X.Msg.Notify("Facturación", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();
            }
            catch (Interfases.Exceptiones.CAppException err)
            {
                X.Mask.Hide();
                X.Msg.Alert("Facturación", err.Mensaje()).Show();
                //Loguear.Error(new Exception(err.Mensaje), this.Usuario.ClaveUsuario);
            }
            catch (Exception err)
            {
                X.Mask.Hide();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                //  X.Msg.Alert("Facturación", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
                X.Msg.Notify("Facturación", err.Message).Show();
                X.Msg.Notify("Facturación", "Comando ejecutado con <br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
            }
        }


        private void BuscaDatosAndBind()
        {
            try
            {
                GridPanel1.GetStore().RemoveAll();

                Int64 ID_Emisor = cmbEmisor.Value == null ? -1 : Int64.Parse(cmbEmisor.Value.ToString());
                Int64 ID_Receptor = cmbReceptor.Value == null ? -1 : Int64.Parse(cmbReceptor.Value.ToString());
                int ID_Estatus = cmbEstatus.Value == null ? -1 : int.Parse(cmbEstatus.Value.ToString());

                DataSet dsFacturas = DAOFactura.ConsultaFacturas(datInicio.SelectedDate, datFinal.SelectedDate,
                    ID_Emisor, ID_Receptor, ID_Estatus, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsFacturas.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Búsqueda de Facturas", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    GridPanel1.GetStore().DataSource = dsFacturas;
                    GridPanel1.GetStore().DataBind();
                }

            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
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
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                Store1.RemoveAll();
                X.Msg.Alert("Búsqueda de Facturas", err.Message).Show();
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
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Facturas", err.Message).Show();
            }

        }

        protected void LlenaEstatus()
        {
            try
            {
                StoreEstatus.RemoveAll();
                StoreEstatus.DataSource = DAOCatalogos.ListaEstatusFactura();//(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaAfiliacion").Valor, cmbSucursal.Value.ToString());
                StoreEstatus.DataBind();

            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Facturas", err.Message).Show();
            }

        }

        protected void LlenaEmisores()
        {
            try
            {

                StoreEmisor.RemoveAll();
                //cmbTerminal.Reset();
                //cmbOperador.Reset();
                StoreEmisor.DataSource = DAOFactura.ListaEmisoresFactura(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreEmisor.DataBind();


            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Factura", err.Message).Show();
            }

        }

        protected void LlenaReceptores()
        {
            try
            {

                StoreReceptor.RemoveAll();
                //cmbTerminal.Reset();
                //cmbOperador.Reset();
                StoreReceptor.DataSource = DAOFactura.ListaReceptoresFactura(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreReceptor.DataBind();


            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Facturas", err.Message).Show();
            }

        }

        protected void EnviarCorreo(object sender, DirectEventArgs e)
        {
            try
            {
                Int64 ID_Factura = (Int64)HttpContext.Current.Session["ID_Factura"];

                Factura LaFacturaToSend = DAOFactura.ObtieneFactura(ID_Factura, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                String laFacturaPDF;

                if (LaFacturaToSend.TipoComprobante.ToUpper().Equals("INGRESO") || LaFacturaToSend.TipoComprobante.ToUpper().Equals("I"))
                {
                    laFacturaPDF = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor + "Factura_" + LaFacturaToSend.Serie + LaFacturaToSend.Folio + ".pdf";
                }
                else
                {
                    laFacturaPDF = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor + "Reporte_" + LaFacturaToSend.Serie + LaFacturaToSend.Folio + ".pdf";
                }

                String LaFacturaXML = "";


                if (LaFacturaToSend.TipoComprobante.ToUpper().Equals("INGRESO") || LaFacturaToSend.TipoComprobante.ToUpper().Equals("I"))
                {
                    // laFacturaPDF = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor + "Factura_" + LaFacturaToSend.Serie + LaFacturaToSend.Folio + ".pdf";
                    LaFacturaXML = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor + "Factura_" + LaFacturaToSend.Serie + LaFacturaToSend.Folio + ".xml";
                }
                else
                {
                    // laFacturaPDF = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor + "Informe_" + LaFacturaToSend.Serie + LaFacturaToSend.Folio + ".pdf";
                    LaFacturaXML = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor + "Reporte_" + LaFacturaToSend.Serie + LaFacturaToSend.Folio + ".xml";
                }






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

                if (LaFacturaToSend.TipoComprobante.ToUpper().Equals("INGRESO") || LaFacturaToSend.TipoComprobante.ToUpper().Equals("I"))
                {
                    Emailing.Send(txtCorreos.Text, ElCuerpodelMail.ToString(), "Factura Emitida por " + LaFacturaToSend.Emisora.NombreORazonSocial, uno, dos);

                    if (uno != null)
                        uno.Dispose();

                    if (dos != null)
                        dos.Dispose();

                    uno = null;
                    dos = null;

                    X.Js.AddScript("#{frmSendMail}.setVisible(false);");

                    Loguear.EntradaSalida("Se envio la Factura [" + LaFacturaToSend.Folio + "] a los Correos: " + txtCorreos.Text, this.Usuario.ClaveUsuario, false);

                    LNFactura.CambiaEstatusFactura(ID_Factura, 6);
                    X.Msg.Notify("Envio de Email", "La Factura se ha enviado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();
                }
                else
                {
                    Emailing.Send(txtCorreos.Text, ElCuerpodelMail.ToString(), "Reporte Emitido por " + LaFacturaToSend.Emisora.NombreORazonSocial, uno, dos);

                    if (uno != null)
                        uno.Dispose();

                    if (dos != null)
                        dos.Dispose();

                    uno = null;
                    dos = null;

                    X.Js.AddScript("#{frmSendMail}.setVisible(false);");

                    Loguear.EntradaSalida("Se envio el Informe [" + LaFacturaToSend.Folio + "] a los Correos: " + txtCorreos.Text, this.Usuario.ClaveUsuario, false);

                    LNFactura.CambiaEstatusFactura(ID_Factura, 8);
                    X.Msg.Notify("Envio de Email", "El Reporte se ha enviado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();

                }

                BuscaDatosAndBind();
            }
            catch (Exception err)
            {
                X.Msg.Notify("Envio de Email", err.Message).Show();
                X.Msg.Notify("Envio de Email", "<br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
            finally
            {
                
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
                    if (lafactura.TipoComprobante.ToUpper().Equals("INGRESO") || lafactura.TipoComprobante.ToUpper().Equals("I"))
                    {
                        String unPath2 = Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor;
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(lafactura.XMLCFDI);
                        xDoc.Save(unPath2 + "Factura_" + lafactura.Serie + lafactura.Folio + ".xml");
                    }
                    else if (lafactura.TipoComprobante.ToUpper().Equals("PAGO") || lafactura.TipoComprobante.ToUpper().Equals("P"))
                    {
                        String unPath2 = Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "urlLasFacturas").Valor;
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(lafactura.XMLCFDI);
                        xDoc.Save(unPath2 + "Pago_" + lafactura.Serie + lafactura.Folio + ".xml");
                    }
                }
                catch (Exception)
                {
                }


                //genera el PDF

                if (lafactura.TipoComprobante.ToUpper().Equals("INGRESO") || lafactura.TipoComprobante.ToUpper().Equals("I"))
                {
                    report.Load(Path2 + lafactura.UrlReporte);
                    Dt = DAOFactura.ObtieneDataSetFactura(lafactura.ID_Factura).Tables[0];
                    report.SetDataSource(Dt);
                    String NombreArchivo = "Factura_" + lafactura.Serie + lafactura.Folio + ".pdf";
                    report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, unPath + NombreArchivo);

                    report.Close();
                } else if (lafactura.TipoComprobante.ToUpper().Equals("PAGO") || (lafactura.TipoComprobante.ToUpper().Equals("P")))
                {
                    report.Load(Path2 + lafactura.UrlReporte);
                    Dt = DAOFactura.ObtieneDataSetFacturaPago(lafactura.ID_Factura).Tables[0];
                    report.SetDataSource(Dt);
                    String NombreArchivo = "Pago_" + lafactura.Serie + lafactura.Folio + ".pdf";
                    report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, unPath + NombreArchivo);

                    report.Close();
                }
                else
                {
                    report.Load(Path2 + lafactura.UrlReporte);
                    Dt = DAOFactura.ObtieneDataSetFactura(lafactura.ID_Factura).Tables[0];
                    report.SetDataSource(Dt);
                    String NombreArchivo = "Reporte_" + lafactura.Serie + lafactura.Folio + ".pdf";
                    report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, unPath + NombreArchivo);

                    report.Close();
                }
                
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

                if (laFactura.TipoComprobante.ToUpper().Equals("INGRESO") || laFactura.TipoComprobante.ToUpper().Equals("I"))
                {
                    NombreArchivo = "Factura_" + laFactura.Serie + laFactura.Folio + ".pdf";
                }
                else if (laFactura.TipoComprobante.ToUpper().Equals("PAGO") || laFactura.TipoComprobante.ToUpper().Equals("P"))
                { 

                    NombreArchivo = "Pago_" + laFactura.Serie + laFactura.Folio + ".pdf";
                 }
                else
                {
                    NombreArchivo = "Reporte_" + laFactura.Serie + laFactura.Folio + ".pdf";
                }
                
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

        /// <summary>
        /// Exporta el grid de resultados al backend, para generar y exportarlo a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            string gridResultJson = e.ExtraParams["GridToExport"];
            string reportName = "MisFacturas";

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
                ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 9).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 10).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 12).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 14).SetDataType(XLCellValues.Number);
            }

            return ws;
        }
    }
}