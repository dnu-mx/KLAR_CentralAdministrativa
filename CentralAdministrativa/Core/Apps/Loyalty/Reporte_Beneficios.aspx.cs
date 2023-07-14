using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentroContacto.LogicaNegocio;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Xml;
using System.Xml.Xsl;

namespace CentroContacto
{
    public partial class Reporte_Beneficios : PaginaBaseCAPP
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

        private void BuscaDatosAndBind()
        {
            try
            {

                String tipoColectiva, sucursal, tipoPromocion;
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
                sucursal = cmbSucursal.Value == null ? null : cmbSucursal.Value.ToString();
                tipoPromocion = txtTipoPromocion.Text.Trim() == "" ? null : txtTipoPromocion.Text.Trim();

                GridPanel1.GetStore().DataSource = LNReportes.ReporteBeneficios(fechaInicio, fechaFin, tipoColectiva, sucursal, tipoPromocion, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
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
    }
}