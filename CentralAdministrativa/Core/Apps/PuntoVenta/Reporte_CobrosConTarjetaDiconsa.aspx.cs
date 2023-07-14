using DALAutorizador.BaseDatos;
using DALCentralAplicaciones;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.LogicaNegocio;
using Ext.Net;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Xml;
using System.Xml.Xsl;

namespace TpvWeb
{
    public partial class Reporte_CobrosConTarjetaDiconsa : PaginaBaseCAPP
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

                    datInicio.SetValue(DateTime.Now);

                    datFinal.SetValue(DateTime.Now);

                    LlenaEstatus();
                    LlenaTiposTarjeta();
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
                String  Estatus, TipoTarjeta, Tarjeta;
                Int32 Sucursal, UnidadOperativa, Almacen, IdTienda;
                DateTime fechaInicial, FechaFinal;


                if (datInicio.Value == null || datFinal.Value == null)
                    return;

                fechaInicial = datInicio.SelectedDate;
                FechaFinal = datFinal.SelectedDate;

                if (fechaInicial > FechaFinal)
                {
                    throw new Exception("La Fecha Inicial es Mayor a la Final.");
                }

                Tarjeta = this.txtTarjeta.Text;

                if (!String.IsNullOrEmpty(Tarjeta))
                {
                    Int32 tarj;

                    if (!Int32.TryParse(Tarjeta, out tarj))
                    {
                        throw new Exception("Los 4 Últimos Dígitos de la Tarjeta deben ser Numéricos.");
                    }
                }


                Sucursal = Convert.ToInt32(cmbSucursal.Value);
                UnidadOperativa = Convert.ToInt32(cmbUnidadOperativa.Value);
                Almacen = Convert.ToInt32(cmbAlmacen.Value);
                IdTienda = Convert.ToInt32(cmbTienda.Value);
                Estatus = cmbEstatus.Value == null ? "" : cmbEstatus.Value.ToString();
                TipoTarjeta = cmbTipoTarjeta.Value == null ? "" : cmbTipoTarjeta.Value.ToString();


                GridPanel1.GetStore().DataSource = LNReportes.ListarCobrosConTarjetaDiconsa(fechaInicial, FechaFinal, 
                    Sucursal, UnidadOperativa, Almacen, IdTienda, TipoTarjeta, Estatus, Tarjeta, 
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
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
                X.Msg.Alert("Cobros con Tarjeta", err.Message).Show();
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
                X.Msg.Alert("Cobros con Tarjeta", err.Message).Show();
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
                cmbTipoTarjeta.Reset();

                StoreUnidadOper.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReporteVentasTienda
                    (DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "ClaveTipoColectivaUnidadOperativaDiconsa").Valor, Int64.Parse(cmbSucursal.Value.ToString()),
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreUnidadOper.DataBind();

            }
            catch (Exception err)
            {
                X.Msg.Alert("Cobros con Tarjeta", err.Message).Show();
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
                X.Msg.Alert("Cobros con Tarjeta", err.Message).Show();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        protected void LlenaTiposTarjeta()
        {
            try
            {
                StoreTipoTarjeta.RemoveAll();

                StoreTipoTarjeta.DataSource = DAOReportes.ListaTiposTarjeta(this.Usuario);

                StoreTipoTarjeta.DataBind();         
            }

            catch (Exception err)
            {
                X.Msg.Alert("Tipos de Tarjeta", err.Message).Show();
            }

        }

        protected void LlenaAlmacenes(object sender, EventArgs e)
        {
            try
            {
                StoreAlmacen.RemoveAll();
                cmbAlmacen.Reset();
                cmbTienda.Reset();
                cmbTipoTarjeta.Reset();

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
                X.Msg.Alert("Cobros con Tarjeta", err.Message).Show();
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
                X.Msg.Alert("Cobros con Tarjeta", err.Message).Show();
            }

        }      
    }
}