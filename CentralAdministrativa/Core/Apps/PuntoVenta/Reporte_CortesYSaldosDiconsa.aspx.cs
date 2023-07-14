using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using DALCentralAplicaciones;
using DALAutorizador.LogicaNegocio;
using System.Configuration;
using DALAutorizador.BaseDatos;
using System.Xml.Xsl;
using System.Xml;
using System.Data;
using System.Text;
using DALPuntoVentaWeb.Utilidades;
using DALPuntoVentaWeb.LogicaNegocio;

namespace TpvWeb
{
    public partial class Reporte_CortesYSaldosDiconsa : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    StoreSucursal.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReporteVentasTienda(
                        DALCentralAplicaciones.Utilidades.Configuracion.Get(
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "ClaveTipoColectivaSucursalDiconsa").Valor, -1, this.Usuario, 
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreSucursal.DataBind();

                    this.cmbCorte.SelectedIndex = 0;
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        private void BuscaDatosAndBind()
        {
            try
            {
                Int32 Corte, Sucursal, UnidadOperativa, Almacen, IdTienda;

                Corte = Convert.ToInt32(cmbCorte.Value);
                Sucursal = Convert.ToInt32(cmbSucursal.Value);
                UnidadOperativa = Convert.ToInt32(cmbUnidadOperativa.Value);
                Almacen = Convert.ToInt32(cmbAlmacen.Value);
                IdTienda = Convert.ToInt32(cmbTienda.Value);

                GridPanel1.GetStore().DataSource = LNReportes.ListarCortesYSaldosDiconsa(
                    Corte, Sucursal, UnidadOperativa, Almacen, IdTienda, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
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
                    xtCsv.Load(Server.MapPath("xslFiles/Csv2.xsl"));
                    xtCsv.Transform(xml, null, Response.OutputStream);

                    break;
            }
            this.Response.End();
        }


     
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                GridPanel1.GetStore().RemoveAll();
                BuscaDatosAndBind();
            }

            catch (Exception err)
            {
                Store1.RemoveAll();
                X.Msg.Alert("Cortes y Saldos", err.Message).Show();
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
                X.Msg.Alert("Cortes y Saldos", err.Message).Show();
            }

        }
        protected void LlenaUnidades(object sender, EventArgs e)
        {
            try
            {
                StoreUnidadOper.RemoveAll();
                cmbUnidadOperativa.Reset();
                cmbAlmacen.Reset();
                cmbTienda.Reset();

                StoreUnidadOper.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReporteVentasTienda
                    (DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "ClaveTipoColectivaUnidadOperativaDiconsa").Valor, Int64.Parse(cmbSucursal.Value.ToString()),
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreUnidadOper.DataBind();

            }
            catch (Exception err)
            {
                X.Msg.Alert("Cortes y Saldos", err.Message).Show();
            }

        }

        protected void LlenaAlmacenes(object sender, EventArgs e)
        {
            try
            {
                StoreAlmacen.RemoveAll();
                cmbAlmacen.Reset();
                cmbTienda.Reset();

                StoreAlmacen.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReporteVentasTienda
                    (DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "ClaveTipoColectivaAlmacenDiconsa").Valor,
                        Int64.Parse(cmbUnidadOperativa.Value.ToString()), this.Usuario, 
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreAlmacen.DataBind();

            }
            catch (Exception err)
            {
                X.Msg.Alert("Cortes y Saldos", err.Message).Show();
            }

        }

        protected void LlenaTiendas(object sender, EventArgs e)
        {
            try
            {
                StoreTienda.RemoveAll();

                StoreTienda.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReporteVentasTienda
                    (DALCentralAplicaciones.Utilidades.Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "ClaveTipoColectivaCadenaComercial").Valor, Int64.Parse(cmbAlmacen.Value.ToString()),
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                StoreTienda.DataBind();
            }

            catch (Exception err)
            {
                X.Msg.Alert("Cortes y Saldos", err.Message).Show();
            }

        }      
    }
}