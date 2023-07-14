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
    public partial class Reporte_Clientes : PaginaBaseCAPP
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

                    StoreTipoMediosAccesos.DataSource = DAOCatalogos.ListaTipoMediosAcceso(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreTipoMediosAccesos.DataBind();

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

                String tipoColectiva, sucursal, tipoMedioAcceso, email;
                DateTime fechaInicio, fechaFin;


                if (datInicio.Value == null || datFinal.Value == null)
                    return;


                fechaInicio = datInicio.SelectedDate;
                fechaFin = datFinal.SelectedDate;

                if (fechaInicio > fechaFin)
                {
                    throw new Exception("La Fecha Inicial es Mayor a la Final.");
                }


                //ID_CadenaComercial = cmbCadenaComercial.Value == null ? -1 : int.Parse(cmbCadenaComercial.Value.ToString());
                tipoColectiva = cmbCadenaComercial.Value == null ? "-1" : cmbCadenaComercial.Value.ToString();
                sucursal = cmbSucursal.Value == null ? "-1" : cmbSucursal.Value.ToString();
                tipoMedioAcceso = cmbTipoMedioAcceso.Value == null ? "-1" : cmbTipoMedioAcceso.Value.ToString();
                email = txtEmail.Text == "" ? null : txtEmail.Text;
                //Estatus = cmbEstatus.Value == null ? "-1" : cmbEstatus.Value.ToString();
                //Benefiario = cmbBeneficiario.Value == null ? "-1" : cmbBeneficiario.Value.ToString();
                //Telefono = txtTelefono.Text.Trim().Length == 0 ? "-1" : txtTelefono.Text.Trim();

                //string format = "yyyy-dd-MM HH:mm:ss";
                //DateTime date = Convert.ToDateTime(DateTime.Now.ToString(format));
                //Afiliacion, Terminal, Operador, Benefiario, Estatus,
                //GridPanel1.GetStore().DataSource = LNOperaciones.ListarOperacionesCadena(DateTime fechaInicio,DateTime fechaFin, int tipoColectiva, int sucursal,string tipoMedioAcceso,string email, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                
                GridPanel1.GetStore().DataSource = LNReportes.ReporteClientes(fechaInicio,fechaFin, tipoColectiva, sucursal,tipoMedioAcceso,email, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
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

        

        //protected void LlenaAfiliaciones(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        StoreAfiliacion.RemoveAll();
        //        cmbAfiliacion.Reset();
        //        cmbTerminal.Reset();
        //        cmbOperador.Reset();

        //        cmbBeneficiario.Reset();
        //        StoreAfiliacion.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaAfiliacion").Valor, cmbSucursal.Value.ToString(), Int64.Parse(cmbCadenaComercial.Value.ToString()));
        //        StoreAfiliacion.DataBind();

        //    }
        //    catch (Exception err)
        //    {
        //        X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
        //    }

        //}

        //protected void LlenaEstatus()
        //{
        //    try
        //    {
        //        StoreEstatus.RemoveAll();
        //        StoreEstatus.DataSource = DAOCatalogos.ListaEstatusOperacion();//(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaAfiliacion").Valor, cmbSucursal.Value.ToString());
        //        StoreEstatus.DataBind();

        //        cmbEstatus.SetValue("1");

        //    }
        //    catch (Exception err)
        //    {
        //        X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
        //    }

        //}

        //protected void LlenaBeneficiario()
        //{
        //    try
        //    {

        //        StoreBeneficiario.RemoveAll();
        //        //cmbTerminal.Reset();
        //        //cmbOperador.Reset();
        //        StoreBeneficiario.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaBeneficiario").Valor, "0", -1);
        //        StoreBeneficiario.DataBind();


        //    }
        //    catch (Exception err)
        //    {
        //        X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
        //    }

        //}

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

        //protected void LlenaTerminales(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        StoreTerminal.RemoveAll();
        //        cmbTerminal.Reset();
        //        cmbOperador.Reset();
        //        cmbBeneficiario.Reset();
        //        StoreTerminal.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaTerminal").Valor, cmbAfiliacion.Value.ToString(), Int64.Parse(cmbCadenaComercial.Value.ToString()));
        //        StoreTerminal.DataBind();

        //    }
        //    catch (Exception err)
        //    {
        //        X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
        //    }

        //}

        //protected void LlenaOperadores()
        //{
        //    try
        //    {
        //        StoreOperador.RemoveAll();
        //        cmbOperador.Reset();

        //        //StoreOperador.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaOperador").Valor, cmbTerminal.Value.ToString());
        //        //StoreOperador.DataBind();

        //        try
        //        {
        //            StoreOperador.DataSource = DAOCatalogos.ListaTiposColectivasPorTipoColectiva(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaOperador").Valor);
        //            StoreOperador.DataBind();
        //        }
        //        catch (Exception err)
        //        {
        //        }

        //    }
        //    catch (Exception err)
        //    {
        //        X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
        //    }

        //}
    }
}