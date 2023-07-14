using DALAutorizador.BaseDatos;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Facturas.LogicaNegocio;
using System;
using System.Configuration;
using System.Data;
using System.Xml;
using System.Xml.Xsl;

namespace Facturas
{
    public partial class AsignarPagoFactura : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                if (!IsPostBack)
                {
                    datFacturaFin.SetValue(DateTime.Now);
                    datFacturaInicio.SetValue(DateTime.Now);
                    datPagoFin.SetValue(DateTime.Now);
                    datPagoInicio.SetValue(DateTime.Now);
                }

            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

        }



        [DirectMethod(Namespace = "Asignaciones", Timeout = 120000)]
        public void btnAsignarPagos()
        {
            try
            {
                RowSelectionModel lasFacturas = this.GridPanel1.SelectionModel.Primary as RowSelectionModel;
                RowSelectionModel lasPolizas = this.GridPanel2.SelectionModel.Primary as RowSelectionModel;

                if ((lasFacturas.SelectedRows.Count > 1) & (lasPolizas.SelectedRows.Count > 1))
                {
                    X.Msg.Alert("Asignación de Pagos", "Solo pueden asignarse un Pago a varias Facturas o una Factura a varios Pagos.").Show();
                    return;
                }


                if ((lasFacturas.SelectedRows.Count == 0) | (lasPolizas.SelectedRows.Count == 0))
                {
                    X.Msg.Alert("Asignación de Pagos", "Elige el Pago y la Factura que deseas asignar").Show();
                    return;
                }




                foreach (SelectedRow laPoliza in lasPolizas.SelectedRows)
                {
                    foreach (SelectedRow laFactura in lasFacturas.SelectedRows)
                    {
                        //result.Append("<li>" + row.RecordID + "</li>");
                        try
                        {
                            LNPagos.AsignarPagoAFactura(Int64.Parse(laFactura.RecordID), Int64.Parse(laPoliza.RecordID), 0, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                            X.Msg.Notify("Asignación de Pagos", "Asginación del Pago [" + laPoliza.RecordID + "] <br /> se asignó a la Factura [" + laFactura.RecordID + "] <br /> <br />  <b> A U T O R I Z A D A </b> <br />  <br /> ").Show();
                        }
                        catch (Exception err)
                        {
                            X.Mask.Hide();
                            Loguear.Error(err, "AsignacionDePagos");
                            X.Msg.Notify("Asignación de Pagos", "Asginación del Pago [" + laPoliza.RecordID + "] <br /> NO SE ASIGNÓ a la Factura [" + laFactura.RecordID + "] <br /> <br />  <b> D E C L I N A D A </b> <br />  <br /> ").Show();
                        }
                    }
                }

                try
                {

                    BindDataFacturaXCadena(datFacturaInicio.SelectedDate, datFacturaFin.SelectedDate, txtCadenaOReceptor.Text, txtFolio.Text);

                    BindDataPagos(datPagoInicio.SelectedDate, datPagoFin.SelectedDate);
                }
                catch (Exception err)
                {
                    X.Mask.Hide();
                    Loguear.Error(err, "AsignacionDePagos");
                }

                X.Mask.Hide();
            }
            catch (Exception err)
            {
                X.Mask.Hide();
                Loguear.Error(err, "AsignacionDePagos");
                X.Msg.Notify("Asignación de Pagos", "Mensaje: <b>" + err.Message + "</b> <br />").Show();
                X.Msg.Notify("Asignación de Pagos", "<br /><br />  <b> D E C L I N A D A </b> <br />  <br /> ").Show();
            }

        }

        protected void btnLimpiarSeleccion(object sender, EventArgs e)
        {
            try
            {
                this.CheckboxSelectionModel1.ClearSelections();
                this.CheckboxSelectionModel2.ClearSelections();
            }
            catch (Exception err)
            {
                Loguear.Error(err, "btnLimpiarSeleccion");
            }
        }


        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                BindDataPagosXCadena(datPagoInicio.SelectedDate, datPagoFin.SelectedDate, txtCadenaComercial.Text);
            }

            catch (Exception err)
            {
                Loguear.Error(err, "AsignacionDePagos");
            }
        }

        protected void btnBuscar2_Click(object sender, EventArgs e)
        {
            try
            {

                BindDataFacturaXCadena(datFacturaInicio.SelectedDate, datFacturaFin.SelectedDate, txtCadenaOReceptor.Text, txtFolio.Text);
            }
            catch (Exception err)
            {
                Loguear.Error(err, "AsignacionDePagos");
            }
        }




        private void BindDataPagos(DateTime FechaInicio, DateTime FechaFin)
        {
            //var store = this.GridPanel1.GetStore();
            this.CheckboxSelectionModel1.ClearSelections();
            this.CheckboxSelectionModel2.ClearSelections();



            GridPanel2.GetStore().DataSource = DAOPoliza.ObtienePagosPendientesAsignar(FechaInicio, FechaFin, this.Usuario.UsuarioTemp, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

            GridPanel2.GetStore().DataBind();


        }

        private void BindDataPagosXCadena(DateTime FechaInicio, DateTime FechaFin, String cadenaOReceptor)
        {
            this.CheckboxSelectionModel1.ClearSelections();
            this.CheckboxSelectionModel2.ClearSelections();

            DataSet dsPagos = DAOPoliza.ObtienePagosPendientesAsignar(FechaInicio, FechaFin,
                cadenaOReceptor, this.Usuario.UsuarioTemp,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

            if (dsPagos.Tables[0].Rows.Count < 1)
            {
                X.Msg.Alert("Búsqueda de Pagos", "No existen coincidencias con la búsqueda solicitada.").Show();
                return;
            }
            else
            {
                GridPanel2.GetStore().DataSource = dsPagos;
                GridPanel2.GetStore().DataBind();
            }
        }

        private void BindDataFactura(DateTime FechaInicio, DateTime FechaFin, String RFC, String Folio)
        {
            //var store = this.GridPanel1.GetStore();
            this.CheckboxSelectionModel1.ClearSelections();
            this.CheckboxSelectionModel2.ClearSelections();

            GridPanel1.GetStore().DataSource = DAOFactura.ObtieneFacturasPendientesPago(FechaInicio, FechaFin, RFC, Folio, this.Usuario.UsuarioTemp, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel1.GetStore().DataBind();


        }

        private void BindDataFacturaXCadena(DateTime FechaInicio, DateTime FechaFin, String cadenaOReceptor, String Folio)
        {
            this.CheckboxSelectionModel1.ClearSelections();
            this.CheckboxSelectionModel2.ClearSelections();

            DataSet dsFacturas = DAOFactura.ObtieneFacturasPendientesPago(
                FechaInicio, FechaFin, cadenaOReceptor, Folio, this.Usuario.UsuarioTemp,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

            if (dsFacturas.Tables[0].Rows.Count < 1)
            {
                X.Msg.Alert("Búsqueda de Facturas", "No existen coincidencias con la búsqueda solicitada.").Show();
                return;
            }
            else
            {
                GridPanel1.GetStore().DataSource = dsFacturas;
                GridPanel1.GetStore().DataBind();
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

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al control de
        /// asignación de pagos a facturas
        /// </summary>
        [DirectMethod(Namespace = "Asignaciones")]
        public void StopMask()
        {
            X.Mask.Hide();
        }
    }
}