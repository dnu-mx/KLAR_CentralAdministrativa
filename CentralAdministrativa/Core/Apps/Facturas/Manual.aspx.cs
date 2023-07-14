using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.LogicaNegocio;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;

namespace Facturas
{
    public partial class Manual : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    StoreTipoColectiva.DataSource = StoreTipoColectiva2.DataSource = 
                        DAOCatalogos.ListaTiposDeColectiva(this.Usuario, 
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreTipoColectiva.DataBind();
                    StoreTipoColectiva2.DataBind();

                    StoreTipoDocto.DataSource = DAOCatalogos.ListaTiposDocumentos();
                    StoreTipoDocto.DataBind();

                    StoreProductos.DataSource = DAOFactura.ListaProductosFacturacion();
                    StoreProductos.DataBind();

                    PreparaGridPropiedades();
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "FACTURACION");
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

        protected void guardarDetalle(object sender, DirectEventArgs e)
        {
            try
            {
                X.Js.AddScript("#{frmAddDetalles}.setVisible(false);");

                X.Msg.Notify("Facturación Tipo", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();

            }
            catch (Exception err)
            {
                Loguear.Error(err, "FACTURACION");
                X.Msg.Notify("Facturación Tipo", err.Message).Show();
                X.Msg.Notify("Facturación Tipo", "Comando ejecutado con <br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
            }
        }

        protected void FileUploadField_FileSelected(object sender, DirectEventArgs e)
        {
            try
            {
                if (FileSelect.HasFile)
                {
                    String URLArchivo = Server.MapPath("../Apps/Facturas/ArchivosProd/") + Path.GetFileName(FileSelect.FileName);
                    this.FileSelect.PostedFile.SaveAs(URLArchivo);
                    
                    //Subir el archivo a la BD
                    List<DetalleFactura> losDetallesdelaFactura = LNFactura.ObtieneDetallesDeArchivo(URLArchivo);

                    StoreDetalleFactura.DataSource = losDetallesdelaFactura;
                    StoreDetalleFactura.DataBind();

                    X.Js.AddScript("#{frmImportFile}.setVisible(false);");
                }
            }
            catch (Exception err)
            {
                X.Msg.Alert("Upload Archivo", "Error al subir el Archivo:" + err.Message).Show();
            }
            finally
            {
              //  FormPanel1.Reset();
            }

        }

        protected void GenerarFactura(object sender, DirectEventArgs e)
        {
            try
            {
                Int32 total = (Int32.Parse(e.ExtraParams["TotalRegistros"]));

                if (total == 0)
                {
                    X.Msg.Notify("Facturación", "La Factura no tiene detalles").Show();
                    X.Msg.Notify("Facturación", "<br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
                    return;
                }


                List<DetalleFactura> losDetalles = new List<DetalleFactura>();

                string json = e.ExtraParams["Values"];

                Dictionary<string, string>[] losProductos = JSON.Deserialize<Dictionary<string, string>[]>(json);


                //Obtiene los valores del Propierty Grid.

                Dictionary<String, ParametroFacturaTipo> losParame = new Dictionary<string, ParametroFacturaTipo>();


                foreach (ParametroFacturaTipo unaProp in DAOFacturaTipo.ListaDeValoresFacturaManual(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Values)
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

                foreach (Dictionary<string, string> row in losProductos)
                {
                    DetalleFactura unNuevoDetalle = new DetalleFactura();

                    foreach (KeyValuePair<string, string> keyValuePair in row)
                    {

                        if (keyValuePair.Key.ToUpper().Equals("CANTIDAD", StringComparison.CurrentCultureIgnoreCase))
                        {
                            unNuevoDetalle.Cantidad = Int32.Parse(keyValuePair.Value);
                        }
                        else if (keyValuePair.Key.ToUpper().Equals("NombreProducto".ToUpper(), StringComparison.CurrentCultureIgnoreCase))
                        {
                            //unNuevoDetalle.NombreProducto = keyValuePair.Value;
                            DAOFactura.ProductosFacturacion(Int32.Parse(keyValuePair.Value), ref unNuevoDetalle);
                        }
                        else if (keyValuePair.Key.ToUpper().Equals("PrecioUnitario".ToUpper(), StringComparison.CurrentCultureIgnoreCase))
                        {
                            unNuevoDetalle.PrecioUnitario = Decimal.Parse(keyValuePair.Value);
                        }
                        else if (keyValuePair.Key.ToUpper().Equals("Unidad".ToUpper(), StringComparison.CurrentCultureIgnoreCase))
                        {
                            unNuevoDetalle.Unidad = keyValuePair.Value;
                        }
                    }

                    unNuevoDetalle.Total = unNuevoDetalle.Cantidad * unNuevoDetalle.PrecioUnitario;
                    unNuevoDetalle.impBase = (unNuevoDetalle.Cantidad * unNuevoDetalle.PrecioUnitario).ToString();
                    unNuevoDetalle.ImporteIva = unNuevoDetalle.Total * decimal.Parse(unNuevoDetalle.impTasaOCuota);

                    losDetalles.Add(unNuevoDetalle);
                }


                Factura lanuevaFactura = LNFactura.CrearFacturaManual(Int64.Parse(cmbColectivaEmisor.SelectedItem.Value), Int64.Parse(cmbColectivaReceptor.SelectedItem.Value), txtDescripcion.Text, cmbTipoComprobante.SelectedItem.Value, losDetalles, losParame, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (lanuevaFactura.ID_Factura != 0)
                {
                    FormPanel1.Reset();
                    GridPanel2.GetStore().RemoveAll();
                    PreparaGridPropiedades();
                    X.Msg.Notify("Facturación", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();
                }
                else
                {
                    throw new Exception("No se puede generar la Factura");
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Notify("Generar Factura", err.Message).Show();
                //X.Msg.Notify("Facturación", "Comando ejecutado con <br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
            }
        }

        protected void consultaEmisores(object sender, DirectEventArgs e)
        {
            try
            {
                StoreEmisor.RemoveAll();

                DataSet dsEmisores = DAOCatalogos.ListaColectivasPorTipo(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    this.cmbTipoColectivaEmisor.SelectedItem.Value);

                List<ColectivaComboPredictivo> ListaEmisores = new List<ColectivaComboPredictivo>();

                foreach (DataRow emisor in dsEmisores.Tables[0].Rows)
                {
                    var ComboEmisor = new ColectivaComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt64(emisor["ID_Colectiva"].ToString()),
                        ClaveColectiva = emisor["ClaveColectiva"].ToString(),
                        NombreORazonSocial = emisor["NombreORazonSocial"].ToString()
                    };
                    ListaEmisores.Add(ComboEmisor);
                }

                StoreEmisor.DataSource = ListaEmisores;
                StoreEmisor.DataBind();
            }

            catch (Exception Error)
            {
                Loguear.Error(Error, "FACTURACION");
            }
        }

        protected void consultaReceptores(object sender, DirectEventArgs e)
        {
            try
            {
                DataSet dsReceptores = DAOCatalogos.ListaColectivasPorTipo(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    this.cmbtipoColectivaReceptor.SelectedItem.Value);

                List<ColectivaComboPredictivo> ListaReceptores = new List<ColectivaComboPredictivo>();

                foreach (DataRow receptor in dsReceptores.Tables[0].Rows)
                {
                    var ComboReceptor = new ColectivaComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt64(receptor["ID_Colectiva"].ToString()),
                        ClaveColectiva = receptor["ClaveColectiva"].ToString(),
                        NombreORazonSocial = receptor["NombreORazonSocial"].ToString()
                    };
                    ListaReceptores.Add(ComboReceptor);
                }

                StoreReceptor.DataSource = ListaReceptores;
                StoreReceptor.DataBind();
            }

            catch (Exception Error)
            {
                Loguear.Error(Error, "FACTURACION");
            }
        }

        protected void PreparaGridPropiedades()
        {
            try
            {
                PropertyGridParameterCollection source = new PropertyGridParameterCollection();
                // populating
                this.GridPropiedades.SetSource(source);


                foreach (ParametroFacturaTipo unaProp in DAOFacturaTipo.ListaDeValoresFacturaManual(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Values)
                {
                    PropertyGridParameter GridProp = new PropertyGridParameter(unaProp.Nombre, unaProp.Valor);

                    if (unaProp.ValorPreestablecido)
                    {
                        DataSet dsValores = DAOFacturaTipo.ObtieneCatalogoValoresConfiguracion(unaProp.ID_Parametro, this.Usuario);

                        ComboBox cb = new ComboBox() { ID = unaProp.Nombre.Replace('@', '_') };

                        for (int valor = 0; valor < dsValores.Tables[0].Rows.Count; valor++)
                        {
                            ListItem li = new ListItem();
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
                                    df.Text = unaProp.Valor;
                                    df.AllowBlank = false;
                                    df.MaxDate = DateTime.Today;
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
            }

            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Prestablece los valores de los parámetros extra de colectiva del tipo
        /// Emisor o Receptor, según lo que se haya seleccionado
        /// </summary>
        /// <param name="esReceptor">Bandera para consultar los parámetros de receptor</param>
        /// <param name="esEmisor">Bandera para consultar los parámetros de emisor</param>
        [DirectMethod(Namespace = "FacturaManual")]
        public void PrestableceParametros(short esReceptor, short esEmisor)
        {
            try
            {
                Int64 IdCadena = Convert.ToBoolean(esReceptor) ?
                    Convert.ToInt64(cmbColectivaReceptor.SelectedItem.Value) :
                    Convert.ToInt64(cmbColectivaEmisor.SelectedItem.Value);

                DataSet dsParamsExtra = DAOFactura.ObtieneValoresExtraColectiva(
                    IdCadena, esReceptor, esEmisor, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                foreach (DataRow dr in dsParamsExtra.Tables[0].Rows)
                {
                    GridPropiedades.UpdateProperty(dr["Nombre"].ToString(), dr["Valor"]);
                }
            }
            
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }
    }
}