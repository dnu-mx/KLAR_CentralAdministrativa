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

namespace TpvWeb
{
    public partial class Reporte_OperacionesCadConcentradoNuevo : PaginaBaseCAPP
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    StoreCadenaComercial.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaCadenaComercial").Valor, "", -1);
                    StoreCadenaComercial.DataBind();

                    datInicio.SetValue(DateTime.Now);

                    datFinal.SetValue(DateTime.Now);

                    LlenaEstatus();
                    LlenaOperadores();
                    LlenaBeneficiario();
                }
            }
            catch (Exception err)
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

                //this.Panel3.Visible = false;

                if (datInicio.Value == null || datFinal.Value == null)
                    return;


                fechaInicial = datInicio.SelectedDate;
                FechaFinal = datFinal.SelectedDate;

                if (fechaInicial > FechaFinal)
                {
                    throw new Exception("La Fecha Inicial es Mayor a la Final.");
                }


                ID_CadenaComercial = cmbCadenaComercial.Value == null ? -1 : int.Parse(cmbCadenaComercial.Value.ToString());
                //Afiliacion = cmbAfiliacion.Value == null ? "-1" : cmbAfiliacion.Value.ToString();
                //Sucursal = cmbSucursal.Value == null ? "-1" : cmbSucursal.Value.ToString();
                //Terminal = cmbTerminal.Value == null ? "-1" : cmbTerminal.Value.ToString();
                //Operador = cmbOperador.Value == null ? "-1" : cmbOperador.Value.ToString();
                //Estatus = cmbEstatus.Value == null ? "-1" : cmbEstatus.Value.ToString();
                Benefiario = cmbBeneficiario.Value == null ? "-1" : cmbBeneficiario.Value.ToString();
                //Telefono = txtTelefono.Text.Trim().Length == 0 ? "-1" : txtTelefono.Text.Trim();

                GridPanel1.GetStore().DataSource = LNOperaciones.ListarOperacionesCadenaConcentrado(fechaInicial, FechaFinal, Benefiario, "1", ID_CadenaComercial, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
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
                BuscaDatosAndBind2();
                BuscaDatosAndBind3();

            }
            catch (Exception err)
            {
                Store1.RemoveAll();
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

        }


        protected void Store2_Submit(object sender, StoreSubmitDataEventArgs e)
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





        private void BuscaDatosAndBind2()
        {
            try
            {

                String Benefiario;
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
                //Afiliacion = cmbAfiliacion.Value == null ? "-1" : cmbAfiliacion.Value.ToString();
                //Sucursal = cmbSucursal.Value == null ? "-1" : cmbSucursal.Value.ToString();
                //Terminal = cmbTerminal.Value == null ? "-1" : cmbTerminal.Value.ToString();
                //Operador = cmbOperador.Value == null ? "-1" : cmbOperador.Value.ToString();
                //Estatus = cmbEstatus.Value == null ? "-1" : cmbEstatus.Value.ToString();
                Benefiario = cmbBeneficiario.Value == null ? "-1" : cmbBeneficiario.Value.ToString();
                //Telefono = txtTelefono.Text.Trim().Length == 0 ? "-1" : txtTelefono.Text.Trim();

                GridPanel2.GetStore().DataSource = LNOperaciones.ListarOperacionesCadenaConcentradoPeriodo(fechaInicial, FechaFinal, Benefiario, "1", ID_CadenaComercial, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                GridPanel2.GetStore().DataBind();

            }
            catch (Exception err)
            {
                throw err;
            }
        }


        private void BuscaDatosAndBind3()
        {
            try
            {

                String Benefiario;
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
                //Afiliacion = cmbAfiliacion.Value == null ? "-1" : cmbAfiliacion.Value.ToString();
                //Sucursal = cmbSucursal.Value == null ? "-1" : cmbSucursal.Value.ToString();
                //Terminal = cmbTerminal.Value == null ? "-1" : cmbTerminal.Value.ToString();
                //Operador = cmbOperador.Value == null ? "-1" : cmbOperador.Value.ToString();
                //Estatus = cmbEstatus.Value == null ? "-1" : cmbEstatus.Value.ToString();
                Benefiario = cmbBeneficiario.Value == null ? "-1" : cmbBeneficiario.Value.ToString();
                //Telefono = txtTelefono.Text.Trim().Length == 0 ? "-1" : txtTelefono.Text.Trim();

                GridPanel3.GetStore().DataSource = LNOperaciones.ListarOperacionesGrupoConcentradoPeriodo(fechaInicial, FechaFinal, Benefiario, "1", ID_CadenaComercial, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                GridPanel3.GetStore().DataBind();

            }
            catch (Exception err)
            {
                throw err;
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                GridPanel1.GetStore().RemoveAll();
                GridPanel2.GetStore().RemoveAll();
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
                //StoreAfiliacion.RemoveAll();
                //cmbAfiliacion.Reset();
                //cmbTerminal.Reset();
                //cmbOperador.Reset();

                cmbBeneficiario.Reset();
                //StoreAfiliacion.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaAfiliacion").Valor, cmbSucursal.Value.ToString(), Int64.Parse(cmbCadenaComercial.Value.ToString()));
                //StoreAfiliacion.DataBind();

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
                StoreEstatus.DataSource = DAOCatalogos.ListaEstatusOperacion();//(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaAfiliacion").Valor, cmbSucursal.Value.ToString());
                StoreEstatus.DataBind();

                //cmbEstatus.SetValue("1");

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

                //StoreSucursal.RemoveAll();
                ////cmbTerminal.Reset();
                ////cmbOperador.Reset();
                //StoreSucursal.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaSucursal").Valor, "0", Int64.Parse(cmbCadenaComercial.Value.ToString()));
                //StoreSucursal.DataBind();


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
                //StoreTerminal.RemoveAll();
                //cmbTerminal.Reset();
                //cmbOperador.Reset();
                cmbBeneficiario.Reset();
                //StoreTerminal.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaTerminal").Valor, cmbAfiliacion.Value.ToString(), Int64.Parse(cmbCadenaComercial.Value.ToString()));
                //StoreTerminal.DataBind();

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
                //StoreOperador.RemoveAll();
                //cmbOperador.Reset();

                ////StoreOperador.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaOperador").Valor, cmbTerminal.Value.ToString());
                ////StoreOperador.DataBind();

                //try
                //{
                //    StoreOperador.DataSource = DAOCatalogos.ListaTiposColectivasPorTipoColectiva(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaOperador").Valor);
                //    StoreOperador.DataBind();
                //}
                //catch (Exception err)
                //{
                //}

            }
            catch (Exception err)
            {
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

        }

    }
}