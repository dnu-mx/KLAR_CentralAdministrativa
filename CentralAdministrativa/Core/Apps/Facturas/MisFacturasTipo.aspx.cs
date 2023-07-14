using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.LogicaNegocio;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace Facturas
{
    public partial class MisFacturasTipo : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //StoreCadenaComercial.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaCadenaComercial").Valor, "", -1);
                    //StoreCadenaComercial.DataBind();

                    StoreFacturasTipo.DataSource = DAOFacturaTipo.ListaFacturasTipo(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreFacturasTipo.DataBind();

                    StoreTipoColectiva.DataSource = DAOCatalogos.ListaTipoColectiva(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreTipoColectiva.DataBind();

                    StoreTipoColectiva2.DataSource = DAOCatalogos.ListaTipoColectiva(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreTipoColectiva2.DataBind();

                    StoreTipoPago.DataSource = DAOCatalogos.ListaFormasPago(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreTipoPago.DataBind();

                    this.stContratos.DataSource = DAOCatalogos.ListaContratos(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.stContratos.DataBind();

                }
            }
            catch (Exception)
            {
            }
        }


        protected void PreparaGridPropiedades(Int64 ID_CadenaComercial, Int64 ID_TipoFactura)
        {
            try
            {
                PropertyGridParameterCollection source = new PropertyGridParameterCollection();
                // populating
                this.GridPropiedades.SetSource(source);


                foreach (ParametroFacturaTipo unaProp in DAOFacturaTipo.ListaDeValores(ID_TipoFactura, ID_CadenaComercial, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Values)
                {
                    PropertyGridParameter GridProp = new PropertyGridParameter(unaProp.Nombre, unaProp.Valor);

                    if (unaProp.ValorPreestablecido)
                    {
                        DataSet dsValores = DAOFacturaTipo.ObtieneCatalogoValoresConfiguracion(unaProp.ID_Parametro, this.Usuario);

                        ComboBox cb = new ComboBox() { ID = unaProp.Nombre.Replace('@', '_') };

                        for (int valor = 0; valor < dsValores.Tables[0].Rows.Count; valor++)
                        {
                            Ext.Net.ListItem li = new Ext.Net.ListItem();
                            if (Boolean.Parse(dsValores.Tables[0].Rows[valor]["usoClave"].ToString()))
                            {
                                li.Value = dsValores.Tables[0].Rows[valor]["Clave"].ToString().Trim();
                            }
                            else
                            {
                                li.Value = dsValores.Tables[0].Rows[valor]["Descripcion"].ToString().Trim();
                            }
                            li.Text = dsValores.Tables[0].Rows[valor]["Descripcion"].ToString().Trim();

                            cb.Items.Add(li);
                        }

                        GridProp.Editor.Add(cb);
                    }
                    else
                    {
                        switch (unaProp.TipoDatoJava.ToUpper())
                        {
                            case "FLOAT":
                                {
                                    TextField tf = new TextField() { ID = unaProp.Nombre.Replace('@', '_') };
                                    tf.Text = unaProp.Valor;
                                    tf.AllowBlank = false;
                                    GridProp.Editor.Add(tf); break;
                                }
                            case "STRING":
                                {
                                    TextField df = new TextField() { ID = unaProp.Nombre.Replace('@', '_') };
                                    df.Text = unaProp.Valor;
                                    df.AllowBlank = false;
                                    GridProp.Editor.Add(df); break;
                                }
                            case "DATETIME":
                                {
                                    DateField df = new DateField() { ID = unaProp.Nombre.Replace('@', '_') };
                                    df.AllowBlank = false;
                                    df.Format = "dd-MM-yyyy";
                                    df.Render();
                                    df.Text = unaProp.Valor.Trim().Length == 0 ? DateTime.Now.ToString("dd-MM-yyyy") : unaProp.Valor.Trim();

                                    
                                    GridProp.Editor.Add(df); break;
                                }
                            case "TIME":
                                {
                                    TimeField df = new TimeField() { ID = unaProp.Nombre.Replace('@', '_') };
                                    df.Text = unaProp.Valor;
                                    df.AllowBlank = false;
                                    GridProp.Editor.Add(df); break;
                                }
                            case "INT":
                                {
                                    SpinnerField df = new SpinnerField() { ID = unaProp.Nombre.Replace('@', '_') };
                                    df.Text = unaProp.Valor;
                                    df.AllowBlank = false;
                                    GridProp.Editor.Add(df); break;
                                }
                            case "MONEY":
                                {
                                    TextField df = new TextField() { ID = unaProp.Nombre.Replace('@', '_') };

                                    df.Text = String.Format("{0:c}", float.Parse(unaProp.Valor));
                                    df.AllowBlank = false;
                                    GridProp.Editor.Add(df); break;

                                }
                        }
                    }

                    GridProp.DisplayName = unaProp.Descripcion;
                    GridPropiedades.AddProperty(GridProp);
                }

                this.GridPropiedades.Render();
            }

            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        private void BuscaDatosAndBind()
        {
            try
            {

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
                GridPanel1.GetStore().RemoveAll();

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

        protected void GenerarFactura(object sender, DirectEventArgs e)
        {
            try
            {
               // GridPanel1.GetStore().RemoveAll();

                int ID_FacturaTipo = int.Parse(HttpContext.Current.Session["ID_FacturaTipo"].ToString());
                int ID_CadenaComercial = int.Parse(HttpContext.Current.Session["ID_CadenaComercial"].ToString());

                Dictionary<String, ParametroFacturaTipo> losParame = new Dictionary<string, ParametroFacturaTipo>();


                foreach (ParametroFacturaTipo unaProp in DAOFacturaTipo.ListaDeValores(ID_FacturaTipo, ID_CadenaComercial, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Values)
                {
                    losParame.Add(unaProp.Nombre, unaProp);
                }

                foreach (PropertyGridParameter param in this.GridPropiedades.Source)
                {
                    if ((param.Value.ToString().Trim().Length == 0) || (param.Value.ToString().Contains('(')) || (param.Value.ToString().Contains(')')))
                    {
                        X.Msg.Alert("Facturacion", "El Parámetro [<b>" + param.Name + "</b>] es Obligatorio").Show();
                        return;
                    }

                    ParametroFacturaTipo unParam = losParame[param.Name];

                    unParam.Valor = param.Value.ToString();
                    losParame[param.Name] = unParam;
                }

                List<Factura> lasFacturaNueva = LNFactura.GeneraFactura(ID_FacturaTipo,  losParame, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));


               StoreFactura.DataSource = lasFacturaNueva;// unaFactura.losDetalles;
                   StoreFactura.DataBind();

               X.Msg.Alert("Facturacion", "Se crearon [<b>" + lasFacturaNueva.Count + "</b>] Facturas").Show();
            }
            catch (GenericalException Error)
            {
                X.Msg.Alert("Facturacion", Error.CodigoError + ": "+ Error.Mensaje ).Show();
            }
            catch (Exception Error)
            {
                X.Msg.Alert("Facturacion", "Error:" + Error.Message).Show();
            }
        }


        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                StoreFacturasTipo.DataSource = DAOFacturaTipo.ListaFacturasTipo(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreFacturasTipo.DataBind();
            }
            catch (Exception)
            {
               
            }
        }

        /// <summary>
        /// Calcula la fecha final del periodo que corresponde a la factura tipo elegida, con base en
        /// la fecha inicial del periodo a facturar ingresado, y valida que sea correcta
        /// </summary>
        /// <param name="clavePeriodo">Clave del periodo de la factura tipo</param>
        /// <param name="_fechaInicial">Fecha inicial ingresada para el periodo a facturar</param>
        /// <param name="_fechaFinal">Fecha final ingresada para el periodo a facturar</param>
        /// <returns>TRUE si a fecha final es correcta, FALSE en caso contrario</returns>
        protected bool ValidaFechaFinal(string clavePeriodo, DateTime _fechaInicial, DateTime _fechaFinal)
        {
            DateTime dtFecha = _fechaInicial;
            int anyo = _fechaInicial.Year;
            int mes = _fechaInicial.Month;
            int dia = _fechaInicial.Day;

            switch (clavePeriodo)
            {
                case "ANUAL":
                case "ANU":
                    dtFecha = _fechaInicial.AddYears(1).AddDays(-1);
                    break;

                case "BIM":
                    dtFecha = _fechaInicial.AddMonths(2);
                    anyo = dtFecha.Year;

                    //Si FechaInicial == Último día del mes de Febrero, 
                    //FechaFinal == Penúltimo día del mes siguiente
                    if (dia == DateTime.DaysInMonth(anyo, mes) && mes == 2)
                    {
                        mes = dtFecha.Month;
                        dia = DateTime.DaysInMonth(anyo, mes) - 1;

                        dtFecha = new DateTime(anyo, mes, dia);
                    }
                    //##CASO ESPECIAL
                    //Si FechaInicial == Penúltimo día de Febrero, 
                    //FechaFinal PUEDE SER 26, 27 ó 29 de Abril
                    else if (dia == DateTime.DaysInMonth(anyo, mes) - 1 && mes == 2)
                    {
                        mes = dtFecha.Month;
                        dia = 26;
                        dtFecha = new DateTime(anyo, mes, dia);

                        if (DateTime.Compare(_fechaFinal, dtFecha) != 0)
                        {
                            dia = 27;
                            dtFecha = new DateTime(anyo, mes, dia);
                            if (DateTime.Compare(_fechaFinal, dtFecha) != 0)
                            {
                                dia = 29;
                                dtFecha = new DateTime(anyo, mes, dia);
                            }
                        }
                    }
                    //Si FechaInicial == 30-12, FechaFinal == 26-2 (a menos que sea bisiesto)
                    else if (dia == 30 && mes == 12)
                    {
                        mes = dtFecha.Month;
                        dia = DateTime.IsLeapYear(anyo) ? 27 : 26;

                        dtFecha = new DateTime(anyo, mes, dia);
                    }
                    //El resto de los días
                    else
                    {
                        dtFecha = dtFecha.AddDays(-1);
                    }

                    break;

                case "MENSUAL":
                case "MEN":
                    dtFecha = _fechaInicial.AddMonths(1);
                    anyo = dtFecha.Year;

                    //Si FechaInicial == Último día del mes de Febrero, 
                    //FechaFinal == Penúltimo día del mes siguiente
                    if (dia == DateTime.DaysInMonth(anyo, mes) && mes == 2)
                    {
                        mes = dtFecha.Month;
                        dia = DateTime.DaysInMonth(anyo, mes) - 1;

                        dtFecha = new DateTime(anyo, mes, dia);
                    }
                    //##CASO ESPECIAL
                    //Si FechaInicial == Penúltimo día de Febrero, 
                    //FechaFinal PUEDE SER 26, 27 ó 29 de Marzo
                    else if (dia == DateTime.DaysInMonth(anyo, mes) - 1 && mes == 2)
                    {
                        mes = dtFecha.Month;
                        dia = 26;
                        dtFecha = new DateTime(anyo, mes, dia);

                        if (DateTime.Compare(_fechaFinal, dtFecha) != 0)
                        {
                            dia = 27;
                            dtFecha = new DateTime(anyo, mes, dia);
                            if (DateTime.Compare(_fechaFinal, dtFecha) != 0)
                            {
                                dia = 29;
                                dtFecha = new DateTime(anyo, mes, dia);
                            }
                        }
                    }
                    //Si FechaInicial == 30-01, FechaFinal == 26-02 (a menos que sea bisiesto)
                    else if (dia == 30 && mes == 1)
                    {
                        mes = dtFecha.Month;
                        dia = DateTime.IsLeapYear(anyo) ? 27 : 26;

                        dtFecha = new DateTime(anyo, mes, dia);
                    }
                    //El resto de los días
                    else
                    {
                        dtFecha = dtFecha.AddDays(-1);
                    }

                    break;

                case "TRI":
                    dtFecha = _fechaInicial.AddMonths(3);
                    anyo = dtFecha.Year;

                    //Si FechaInicial == Último día del mes de Febrero, 
                    //FechaFinal == Penúltimo día del mes siguiente
                    if (dia == DateTime.DaysInMonth(anyo, mes) && mes == 2)
                    {
                        mes = dtFecha.Month;
                        dia = DateTime.DaysInMonth(anyo, mes) - 1;

                        dtFecha = new DateTime(anyo, mes, dia);
                    }
                    //##CASO ESPECIAL
                    //Si FechaInicial == Penúltimo día de Febrero, 
                    //FechaFinal PUEDE SER 26, 27 ó 29 de Mayo
                    else if (dia == DateTime.DaysInMonth(anyo, mes) - 1 && mes == 2)
                    {
                        mes = dtFecha.Month;
                        dia = 26;
                        dtFecha = new DateTime(anyo, mes, dia);

                        if (DateTime.Compare(_fechaFinal, dtFecha) != 0)
                        {
                            dia = 27;
                            dtFecha = new DateTime(anyo, mes, dia);
                            if (DateTime.Compare(_fechaFinal, dtFecha) != 0)
                            {
                                dia = 29;
                                dtFecha = new DateTime(anyo, mes, dia);
                            }
                        }
                    }
                    //Si FechaInicial == 30-11, FechaFinal == 26-2 (a menos que sea bisiesto)
                    else if (dia == 30 && mes == 11)
                    {
                        mes = dtFecha.Month;
                        dia = DateTime.IsLeapYear(anyo) ? 27 : 26;

                        dtFecha = new DateTime(anyo, mes, dia);
                    }
                    //El resto de los días
                    else
                    {
                        dtFecha = dtFecha.AddDays(-1);
                    }

                    break;
            }

            return (DateTime.Compare(_fechaFinal, dtFecha) != 0) ? false : true;
        }

        /// <summary>
        /// Valida si el periodo de la factura tipo está entre los que pueden ser adelantados o vencidos,
        /// y de ser así, asegura que se hayan ingresado valores correctos a todos los parámetros de fecha
        /// </summary>
        protected void ValidaFechasPeriodosEspeciales()
        {
            try
            {
                DateTime laFechaDeEmision = new DateTime();
                DateTime laFechaInicial = new DateTime();
                DateTime laFechaFinal = new DateTime();

                DataTable dtPeriodos = DAOFacturaTipo.ListaPeriodosAdelantadosVencidos(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                DataRow[] filteredRows = dtPeriodos.Select("ID_Periodo = " + hdnIdPeriodo.Value.ToString());

                if (filteredRows.Length > 0)
                {
                    foreach (PropertyGridParameter _unParametro in this.GridPropiedades.Source)
                    {
                        if (_unParametro.Name.ToUpper().Equals("@FechaEmision".ToUpper()))
                        {
                            try
                            {
                                laFechaDeEmision = DateTime.Parse(_unParametro.Value.ToString());
                            }
                            catch (Exception)
                            {
                                throw new Exception("El valor ingresado como Fecha de Emisión no es válido");
                            }
                        }

                        if (_unParametro.Name.ToUpper().Equals("@FechaInicial".ToUpper()))
                        {
                            try
                            {
                                laFechaInicial = DateTime.Parse(_unParametro.Value.ToString());
                            }
                            catch (Exception)
                            {
                                throw new Exception("El valor ingresado como Fecha Inicial no es válido");
                            }
                        }

                        if (_unParametro.Name.ToUpper().Equals("@FechaFinal".ToUpper()))
                        {
                            try
                            {
                                laFechaFinal = DateTime.Parse(_unParametro.Value.ToString());
                            }
                            catch (Exception)
                            {
                                throw new Exception("El valor ingresado como Fecha Final no es válido");
                            }
                        }
                    }

                    if ((laFechaDeEmision > laFechaInicial) && (laFechaDeEmision < laFechaFinal))
                    {
                        throw new Exception("La Fecha de Emisión debe ser menor o igual a la Fecha Inicial del Periodo a Facturar, o mayor o igual a la Fecha Final del Periodo a Facturar");
                    }

                    if (!ValidaFechaFinal(filteredRows[0].ItemArray[1].ToString().Trim(), laFechaInicial, laFechaFinal))
                    {
                        throw new Exception("El valor ingresado como Fecha Final no corresponde al Periodo de la Factura Tipo");
                    }
                }
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                throw ex;
            }
        }


        protected void GuardarFactura(object sender, DirectEventArgs e)
        {
            try
            {
                int ID_FacturaTipo = int.Parse(HttpContext.Current.Session["ID_FacturaTipo"].ToString());
                int ID_CadenaComercial = int.Parse(HttpContext.Current.Session["ID_CadenaComercial"].ToString());

                ValidaFechasPeriodosEspeciales();
                
                Dictionary<String, ParametroFacturaTipo> losParame = new Dictionary<string, ParametroFacturaTipo>();

                foreach (ParametroFacturaTipo unaProp in DAOFacturaTipo.ListaDeValores(ID_FacturaTipo, ID_CadenaComercial, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Values)
                {
                    losParame.Add(unaProp.Nombre, unaProp);
                }

                foreach (PropertyGridParameter param in this.GridPropiedades.Source)
                {
                    if ((param.Value.ToString().Trim().Length == 0) || (param.Value.ToString().Contains('(')) || (param.Value.ToString().Contains(')')))
                    {
                        X.Msg.Alert("Facturacion", "El Parámetro [<b>" + param.Name + "</b>] es Obligatorio").Show();
                        return;
                    }

                    ParametroFacturaTipo unParam = losParame[param.Name];

                    unParam.Valor = param.Value.ToString();
                    losParame[param.Name] = unParam;
                }

                List<Factura> lasFacturaNueva = LNFactura.GeneraFactura(ID_FacturaTipo, losParame, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (lasFacturaNueva.Count > 0)
                {
                    StoreFactura.DataSource = lasFacturaNueva;// unaFactura.losDetalles;
                    StoreFactura.DataBind();

                    X.Msg.Notify("Facturación", "Se generaron <br/> <b>" + lasFacturaNueva.Count + "</b> <br/> Facturas").Show();
                    foreach (Factura unaFactura in lasFacturaNueva)
                    {

                        X.Msg.Notify("Facturación", " <br />  <br /> Factura   <b> " + unaFactura.ID_Factura + "<br /> G E N E R A D A  </b> <br />  <br /> ").Show();
                    }
                }
                else
                {
                    X.Msg.Notify("Facturación", "No se generó nunguna Factura ").Show();
                    X.Msg.Notify("Facturación", " <br />  <br /> <b>  D E C L I N A D O  </b> <br />  <br /> ").Show();
                }

            }
            catch (Exception Error)
            {
                X.Msg.Alert("Facturación", Error.Message).Show();
                X.Msg.Notify("Facturación", "No se generó la Factura").Show();
                X.Msg.Notify("Facturación", " <br />  <br /> <b>  D E C L I N A D O  </b> <br />  <br /> ").Show();
            }
        }

        protected void consultaReceptores(object sender, DirectEventArgs e)
        {
            try
            {
                //StoreReceptor.DataSource = DAOCatalogos.ListaTiposColectivasPorTipoColectivaFacturacion(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), this.cmbtipoColectivaReceptor.SelectedItem.Value);
                //StoreReceptor.DataBind();
            }
            catch (Exception)
            {
            }
        }


        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {
                Int64 ID_Factura = Int64.Parse(e.ExtraParams["ID"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                switch (EjecutarComando)
                {
                    case "Detalles":
                        //VEr DETALLES

                        StoreDetalleFactura.DataSource = DAOFactura.ObtieneDetallesFactura(ID_Factura);
                        StoreDetalleFactura.DataBind();
                        //Asginar el email del Receptor como envio principal.
                        

                        winDetallesFactura.Visible = true;
                        winDetallesFactura.Hidden = false;

                        return;
                }

                

               // X.Msg.Notify("Facturación", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();
            }
            catch (Interfases.Exceptiones.CAppException err)
            {
                X.Msg.Alert("Facturación", err.Mensaje()).Show();
            }
            catch (Exception err)
            {
                //  X.Msg.Alert("Facturación", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
                X.Msg.Notify("Facturación", err.Message).Show();
                X.Msg.Notify("Facturación", "Comando ejecutado con <br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
            }

        }

      

        protected void RowSelect(object sender, DirectEventArgs e)
        {
            try
            {
                string ID_FacturaTipo = e.ExtraParams["ID_FacturaTipo"];
                string ID_CadenaComercial = e.ExtraParams["ID_CadenaComercial"];
                hdnIdPeriodo.Value = Int32.Parse(e.ExtraParams["ID_Periodo"]);

                HttpContext.Current.Session.Add("ID_FacturaTipo", ID_FacturaTipo);
                HttpContext.Current.Session.Add("ID_CadenaComercial", ID_CadenaComercial);

                PreparaGridPropiedades( Int64.Parse(ID_CadenaComercial), Int64.Parse(ID_FacturaTipo));

                  GridPanel2.GetStore().RemoveAll();
                
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

           
        }

        protected void Store1_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            this.Store1.DataBind();
        }

    }
}