using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace Facturas
{
    public partial class addFacturaTipo : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    StoreTipoColectiva.DataSource = DAOCatalogos.ListaTipoColectiva(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreTipoColectiva.DataBind();

                    StoreTipoColectiva2.DataSource = DAOCatalogos.ListaTipoColectiva(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreTipoColectiva2.DataBind();

                    stEventosManuales.DataSource = DAOCatalogos.ListaEventos(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    stEventosManuales.DataBind();

                    StorePeriodo.DataSource = DAOCatalogos.ListaPeriodos(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StorePeriodo.DataBind();

                    StoreTipoDocto.DataSource = DAOCatalogos.ListaTiposDocumentos();
                    StoreTipoDocto.DataBind();
                    //this.stContratos.DataSource = DAOCatalogos.ListaContratos(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    //this.stContratos.DataBind();

                }
            }
            catch (Exception)
            {
            }
        }


        protected void PreparaGridPropiedades(Int64 ID_TipoFactura)
        {
            try
            {
                //Hacer visible la pagina de paramentros
                frmParametros.Visible = true;
                frmParametros.Hidden = false;

                PropertyGridParameterCollection source = new PropertyGridParameterCollection();
                // populating
                this.GridPropiedades.SetSource(source);


                foreach (ParametroFacturaTipo unaProp in DAOFacturaTipo.ListaDeValoresConfiguracion(ID_TipoFactura, 0,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Values)
                {
                    PropertyGridParameter GridProp = new PropertyGridParameter(unaProp.Descripcion, unaProp.Valor);

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
                                    // df.Format = "dd-MMM-yyyy";
                                    //df.Text = unaProp.Valor.Trim().Length == 0 ? DateTime.Now.ToString(): unaProp.Valor.Trim();
                                    df.Format = "dd-MM-yyyy";
                                    df.AllowBlank = false;
                                    df.Render();
                                    df.Text = unaProp.Valor.Trim().Length == 0 ? DateTime.Now.ToString("dd-MM-yyyy") : unaProp.Valor.Trim();
                                                                        
                                    GridProp.Editor.Add(df);
                                    
                                    break;
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
                    GridProp.Name = unaProp.Nombre;

                    GridPropiedades.AddProperty(GridProp);

                }

                this.GridPropiedades.Render();

            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
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

        protected void Limpiar_Click(object sender, EventArgs e)
        {
            FormPanel2.Reset();

            txtBusqueda.Reset();

            StoreFacturasTipo.RemoveAll();
            StoreDetalleFacturaTipo.RemoveAll();
        }


        protected void Guardar_Click(object sender, EventArgs e)
        {
            try
            {
                FacturaTipo laNuevaFactura = new FacturaTipo();
                
                laNuevaFactura.ID_ColectiraReceptora = cmbColectivaReceptor.Value == null ? -1 : Int64.Parse(cmbColectivaReceptor.Value.ToString());
                laNuevaFactura.ID_ColectivaEmisora = cmbColectivaEmisor.Value == null ? -1 : Int64.Parse(cmbColectivaEmisor.Value.ToString());
                laNuevaFactura.ID_TipoColectivaNivelDatos = cmbNivelDatos.Value == null ? -1 : Int32.Parse(cmbNivelDatos.Value.ToString());
                laNuevaFactura.ID_TipoColectivaReceptor = cmbNivelReceptor.Value == null ? -1 : Int32.Parse(cmbNivelReceptor.Value.ToString());
                laNuevaFactura.Descripcion=txtNombreFacturaTipo.Text;
                laNuevaFactura.ID_FacturaTipo = Int64.Parse(txtIDFac.Text.Trim().Length == 0 ? "0" : txtIDFac.Text);
                laNuevaFactura.ID_Periodo = CmbPeriodo.Value == null ? -1 : Int32.Parse(CmbPeriodo.Value.ToString());
                laNuevaFactura.ID_TipoDocumento = cmbTipoFactura.Value == null ? -1 : Int32.Parse(cmbTipoFactura.Value.ToString());
                laNuevaFactura.Foliada = Convert.ToBoolean(cmbFolio.SelectedItem.Value);
                

                if (txtIDFac.Text.Trim().Length == 0)
                {
                    //guardar

                    if (DAOFacturaTipo.InsertaFacturaTipo(laNuevaFactura) != 0)
                    {
                        throw new Exception("No se insertó la Factura");
                    }

                    StoreFacturasTipo.RemoveAll();
                    StoreDetalleFacturaTipo.RemoveAll();
                    StoreFacturasTipo.DataSource = DAOFacturaTipo.ObtieneUltimasFacturasTipo(0,this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    StoreFacturasTipo.DataBind();
                   
                }
                else
                {
                    //actualizar
                    if (DAOFacturaTipo.EditarFacturaTipo(laNuevaFactura) != 0)
                    {
                        throw new Exception("No se Actualizo la Factura");
                    }

                    StoreFacturasTipo.RemoveAll();
                    StoreDetalleFacturaTipo.RemoveAll();
                    StoreFacturasTipo.DataSource = DAOFacturaTipo.ObtieneUltimasFacturasTipo(laNuevaFactura.ID_FacturaTipo, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    StoreFacturasTipo.DataBind();
                }

              
            

                FormPanel2.Reset();

                X.Msg.Notify("Facturas Tipo", "Factura Generada con <br />  <br /> <b>E X I T O </b> <br />  <br /> ").Show();

            }
            catch (Exception err)
            {
                StoreFacturasTipo.RemoveAll();
                StoreDetalleFacturaTipo.RemoveAll();
                X.Msg.Notify("Facturas Tipo", err.Message).Show();
                X.Msg.Notify("Facturas Tipo", "<br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
            }

        }

        protected void btnNuevaFactura_Click(object sender, EventArgs e)
        {
            try
            {
                winFacturaTipo.Visible = true;
                winFacturaTipo.Hidden = false;
            }
            catch (Exception err)
            {
                StoreFacturasTipo.RemoveAll();
                StoreDetalleFacturaTipo.RemoveAll();
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

        }


        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                StoreFacturasTipo.RemoveAll();

                DataSet dsFacturasTipo = DAOFacturaTipo.ObtieneFacturasTipoPorBusqueda(txtBusqueda.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsFacturasTipo.Tables[0].Rows.Count < 1)
                {
                    X.Msg.Alert("Búsqueda de Facturas Tipo", "No existen coincidencias con la búsqueda solicitada.").Show();
                    return;
                }
                else
                {
                    StoreFacturasTipo.DataSource = dsFacturasTipo;
                    StoreFacturasTipo.DataBind();
                }
            }

            catch (Exception err)
            {
                StoreFacturasTipo.RemoveAll();
                StoreDetalleFacturaTipo.RemoveAll();
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                GridPanel1.GetStore().RemoveAll();
                FormPanel12.Reset();

            }
            catch (Exception err)
            {
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
            }

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

                DataRow[] filteredRows = dtPeriodos.Select("ID_Periodo = " + this.hdnIdPeriodo.Value.ToString());

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
                throw ex;
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
                    if (dia == DateTime.DaysInMonth(anyo, mes) &&  mes == 2)
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


        protected void btnGuardarParametros_Click(object sender, EventArgs e)
        {
            try
            {
                List<ValorRegla> losCambios = new List<ValorRegla>();

                Int32 ID_FacturaTipo = txtFacturaTipoID.Text.Trim().Length == 0 ? 0 : Int32.Parse(txtFacturaTipoID.Text.Trim());

                ValidaFechasPeriodosEspeciales();                

                //Obtiene las propiedades que cambiaron
                foreach (PropertyGridParameter param in this.GridPropiedades.Source)
                {
                    if (param.IsChanged)
                    {
                        ValorRegla unaProp = new ValorRegla() { Nombre = param.Name, Valor = param.Value.ToString() };

                        losCambios.Add(unaProp);
                    }
                }

                //Guardar Valores
                foreach (ValorRegla unValor in losCambios)
                {
                    DAOFacturaTipo.ModicarValorFacturaTipo(unValor, ID_FacturaTipo, this.Usuario);
                }

                PreparaGridPropiedades(ID_FacturaTipo);

                txtFacturaTipoID.Text = "";

                X.Msg.Notify("Configuración de Parámetros", "Modificación de Valores de la Factura Tipo <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();

                frmParametros.Hide();
            }

            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Configuración de Parámetros", err.Message).Show();
            }
        }


        protected void consultaEmisores(object sender, DirectEventArgs e)
        {
            try
            {
                DataSet dsEmisores = DAOCatalogos.ListaTiposColectivasPorID_TipoColectivaFacturacion(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), Int32.Parse(this.cmbTipoColectivaEmisor.SelectedItem.Value));

                List<ColectivaComboPredictivo> ListaEmisores = new List<ColectivaComboPredictivo>();

                foreach (DataRow emisor in dsEmisores.Tables[0].Rows)
                {
                    var ComboEmisores = new ColectivaComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt64(emisor["ID_Colectiva"].ToString()),
                        ClaveColectiva = emisor["ClaveColectiva"].ToString(),
                        NombreORazonSocial = emisor["NombreORazonSocial"].ToString(),
                        NameFin = emisor["NameFin"].ToString()
                    };
                    ListaEmisores.Add(ComboEmisores);
                }

                StoreEmisor.DataSource = ListaEmisores;
                StoreEmisor.DataBind();
            }
            catch (Exception)
            {
            }
        }

        protected void consultaReceptores(object sender, DirectEventArgs e)
        {
            try
            {
                DataSet dsReceptores = DAOCatalogos.ListaTiposColectivasPorID_TipoColectivaFacturacion(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), Int32.Parse(this.cmbtipoColectivaReceptor.SelectedItem.Value));

                List<ColectivaComboPredictivo> ListaReceptores = new List<ColectivaComboPredictivo>();

                foreach (DataRow receptor in dsReceptores.Tables[0].Rows)
                {
                    var ComboReceptores = new ColectivaComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt64(receptor["ID_Colectiva"].ToString()),
                        ClaveColectiva = receptor["ClaveColectiva"].ToString(),
                        NombreORazonSocial = receptor["NombreORazonSocial"].ToString(),
                        NameFin = receptor["NameFin"].ToString()
                    };
                    ListaReceptores.Add(ComboReceptores);
                }

                StoreReceptor.DataSource = ListaReceptores;
                StoreReceptor.DataBind();
            }
            catch (Exception)
            {
            }
        }

   

        protected void RowSelect(object sender, DirectEventArgs e)
        {
            try
            {
                Int64 ID_FacturaTipo = (Int64.Parse(e.ExtraParams["ID_FacturaTipo"]));
                

                HttpContext.Current.Session.Add("ID_FacturaTipo", ID_FacturaTipo);

                StoreDetalleFacturaTipo.RemoveAll();
                StoreDetalleFacturaTipo.DataSource = DAOFacturaTipo.ObtieneDetallesFacturasTipo(ID_FacturaTipo, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreDetalleFacturaTipo.DataBind();

            }
            catch (Exception err)
            {
                StoreDetalleFacturaTipo.RemoveAll();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }


        }




        protected void EjecutarComandoDetalle(object sender, DirectEventArgs e)
        {

            try
            {
               
             
                String EjecutarComando = (String)e.ExtraParams["Comando"];
                DetalleFacturaTipo unDetalleAEditar = new DetalleFacturaTipo();

                unDetalleAEditar.ID_DetalleFacturaTipo = Int64.Parse(e.ExtraParams["ID_DetalleFacturaTipo"]);
                unDetalleAEditar.ID_FacturaTipo = Int32.Parse(e.ExtraParams["ID_FacturaTipo"]);

                switch (EjecutarComando)
                {
                   
                    case "EditDetalle":
                        SeleccionaCadenaParaDetalle(unDetalleAEditar.ID_FacturaTipo);
                         
                        unDetalleAEditar.FormulaCantidad = e.ExtraParams["FormulaCantidad"];
                        unDetalleAEditar.FormulaPrecioUnitario = e.ExtraParams["FormulaPrecioUnitario"];
                        unDetalleAEditar.FormulaTotal = e.ExtraParams["FormulaTotal"];
                        unDetalleAEditar.ID_Evento = Int32.Parse(e.ExtraParams["ID_Evento"]);
                        unDetalleAEditar.ID_TipoCuenta = Int32.Parse(e.ExtraParams["ID_TipoCuenta"]);
                        unDetalleAEditar.ID_CadenaComercial = Int64.Parse(e.ExtraParams["ID_CadenaComercial"]);
                        

                        MuestraEditarDetalle(unDetalleAEditar);
                        break;
                  
                    case "DeleteDetalleFacturaTipo":
                        Int64 ID_DetalleFacturaTipo = Int64.Parse(e.ExtraParams["ID_DetalleFacturaTipo"]);

                        DAOFacturaTipo.EliminarDetalleFacturaTipo(ID_DetalleFacturaTipo);

                        X.Msg.Notify("Facturación Tipo", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();
                        break;
                  
                }

                
                ActualizaDetalles(unDetalleAEditar.ID_FacturaTipo);
                
            }
            catch (Interfases.Exceptiones.CAppException err)
            {
                X.Msg.Alert("Facturación Tipo", err.Mensaje()).Show();
            }
            catch (Exception err)
            {
                //  X.Msg.Alert("Facturación", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
                X.Msg.Notify("Facturación Tipo", err.Message).Show();
                X.Msg.Notify("Facturación Tipo", "Comando ejecutado con <br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
            }

        }

        protected void ActualizaDetalles(Int64 ID_FacturaTipo)
        {
            StoreDetalleFacturaTipo.RemoveAll();
            StoreDetalleFacturaTipo.DataSource = DAOFacturaTipo.ObtieneDetallesFacturasTipo(ID_FacturaTipo, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
            StoreDetalleFacturaTipo.DataBind();
        }

        protected void guardarDetalle(object sender, DirectEventArgs e)
        {
            try
            {

                DetalleFacturaTipo unDetalleAEditar = new DetalleFacturaTipo();

                unDetalleAEditar.FormulaCantidad = txtformulaCantidad.Text;
                unDetalleAEditar.FormulaPrecioUnitario = txtformulaPrecioUnitario.Text;
                unDetalleAEditar.FormulaTotal = txtformulaTotalDetalle.Text;
                unDetalleAEditar.ID_Evento = Int32.Parse(cmbEventos.Value==null? "0": cmbEventos.Value.ToString());
                unDetalleAEditar.ID_CadenaComercial = Int64.Parse(cmbCadenaComercial.Value == null ? "0" : cmbCadenaComercial.Value.ToString());
                unDetalleAEditar.ID_DetalleFacturaTipo = Int64.Parse(txtID_DetalleFacturaTipo.Text.Trim().Length == 0 ? "0" : txtID_DetalleFacturaTipo.Text);
                unDetalleAEditar.ID_TipoCuenta = Int32.Parse(cmbTipoCuenta.Value == null ? "0" : cmbTipoCuenta.Value.ToString());

                

                if (unDetalleAEditar.ID_Evento == 0)
                {
                    throw new Exception( "Evento Invalido");
                }
                else if (unDetalleAEditar.ID_CadenaComercial == 0)
                {
                    throw new Exception("Cadena Comercial Inválida");
                }



                if (txtID_DetalleFacturaTipo.Text.Trim().Length == 0)
                {
                    //guardar
                    unDetalleAEditar.ID_FacturaTipo = Int32.Parse( txtIDFacturaTipo.Text.Trim().Length == 0 ? "0" : txtIDFacturaTipo.Text);
                    if (DAOFacturaTipo.InsertaDetalleFacturaTipo(unDetalleAEditar) != 0)
                    {
                        throw new Exception("No se insertó el Detalle");
                    }
                }
                else
                {
                    //actualizar
                    if (DAOFacturaTipo.ActualizaDetalleFacturaTipo(unDetalleAEditar) != 0)
                    {
                        throw new Exception("No se Actualizo el Detalle");
                    }
                }

                frmNuevo.Reset();
                ActualizaDetalles(unDetalleAEditar.ID_FacturaTipo);
                X.Js.AddScript("#{frmAddDetalles}.setVisible(false);");

                X.Msg.Notify("Facturación Tipo", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();

            }
            catch (Exception err)
            {
                X.Msg.Notify("Facturación Tipo", err.Message).Show();
                X.Msg.Notify("Facturación Tipo", "Comando ejecutado con <br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
            }
        }


        protected void SeleccionaCadenaParaDetalle(Int64 ID_FacturaTipo)
        {
            try
            {
                DataSet dsCadenas = DAOCatalogos.ObtieneColectivasCadenaComercialFacturaTipo(ID_FacturaTipo, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                List<ColectivaComboPredictivo> ListaCadenas = new List<ColectivaComboPredictivo>();

                foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
                {
                    var ComboCadenas = new ColectivaComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt64(cadena["ID_Colectiva"].ToString()),
                        ClaveColectiva = cadena["ClaveColectiva"].ToString(),
                        NombreORazonSocial = cadena["NombreORazonSocial"].ToString()
                    };
                    ListaCadenas.Add(ComboCadenas);
                }

                StoreCadenaComercial.DataSource = ListaCadenas;
                StoreCadenaComercial.DataBind();


                //agrega los tipos de cuenta.
                StoreTipoCuenta.DataSource = DAOCuenta.ObtieneTiposCuenta(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreTipoCuenta.DataBind();
            }
            catch (Exception)
            {
            }
        }

        protected void EjecutarComandoFactura(object sender, DirectEventArgs e)
        {

            try
            {
                Int64 ID_FacturaTipo = Int64.Parse(e.ExtraParams["ID_FacturaTipo"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                HttpContext.Current.Session.Add("ID_Factura", ID_FacturaTipo);
                
                switch (EjecutarComando)
                {
                    case "AddDetalle":
                        txtIDFacturaTipo.Text = ID_FacturaTipo.ToString();

                        SeleccionaCadenaParaDetalle(ID_FacturaTipo);
                        frmAddDetalles.Visible = true;
                        frmAddDetalles.Hidden = false;

                        break;
                    case "DeleteFacturaTipo":

                        if (DAOFacturaTipo.EliminarFacturaTipo(ID_FacturaTipo) != 0)
                        {
                            throw new Exception("No se elimino la factura");
                        }

                        StoreFacturasTipo.RemoveAll();
                        StoreFacturasTipo.DataSource = DAOFacturaTipo.ObtieneFacturasTipoPorBusqueda(txtBusqueda.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                        StoreFacturasTipo.DataBind();

                        X.Msg.Notify("Facturación Tipo", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();
                        break;
                    case "EditFacturaTipo":
                        
                      //  txtIDFacTipo.Text = ID_FacturaTipo.ToString();
                        
                        FacturaTipo unaFacturaTipo = new FacturaTipo();

                        unaFacturaTipo.Descripcion = e.ExtraParams["Descripcion"];
                        unaFacturaTipo.ID_TipoColectivaEM = Int32.Parse(e.ExtraParams["ID_TipoColectivaEM"]);
                        unaFacturaTipo.ID_TipoColectivaRE = Int32.Parse(e.ExtraParams["ID_TipoColectivaRE"]);
                        unaFacturaTipo.ID_TipoColectivaReceptor = Int32.Parse(e.ExtraParams["ID_TipoColectivaReceptor"]);
                        unaFacturaTipo.ID_TipoColectivaNivelDatos = Int32.Parse(e.ExtraParams["ID_TipoColectivaNivelDatos"]);
                        unaFacturaTipo.ID_FacturaTipo = Int64.Parse(e.ExtraParams["ID_FacturaTipo"]);
                        unaFacturaTipo.ID_ColectivaEmisora = Int64.Parse(e.ExtraParams["ID_Emisor"]);
                        unaFacturaTipo.ID_ColectiraReceptora = Int64.Parse(e.ExtraParams["ID_Receptor"]);
                        unaFacturaTipo.ID_TipoDocumento = Int32.Parse(e.ExtraParams["ID_TipoDocumento"]);
                        unaFacturaTipo.Foliada = bool.Parse(e.ExtraParams["Foliada"]);

                        try
                        {
                            unaFacturaTipo.ID_Periodo = Int32.Parse(e.ExtraParams["ID_Periodo"]);
                        }
                        catch (Exception)
                        {
                        }

                        MuestraEditarFacturaTipo(unaFacturaTipo);
                        break;

                    case "ConfigParametros":
                        int IdPeriodo;

                        if (!Int32.TryParse(e.ExtraParams["ID_Periodo"], out IdPeriodo))
                        {
                            X.Msg.Alert("Facturación Tipo", "La Factura Tipo no tiene un periodo válido " +
                                " de generación automática. Por favor, verifícalo en el encabezado.").Show();
                            return;
                        }
                        
                        this.hdnIdPeriodo.Value = IdPeriodo;
                        this.txtFacturaTipoID.Text = ID_FacturaTipo.ToString();

                        PreparaGridPropiedades(ID_FacturaTipo);
                        break;
                }
            }

            catch (Interfases.Exceptiones.CAppException err)
            {
                X.Msg.Alert("Facturación Tipo", err.Mensaje()).Show();
            }

            catch (Exception err)
            {
                //  X.Msg.Alert("Facturación", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
                X.Msg.Notify("Facturación Tipo", err.Message).Show();
                X.Msg.Notify("Facturación Tipo", "Comando ejecutado con <br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
            }
            finally
            {
                Thread.Sleep(5000);
                X.Mask.Hide();
            }
        }


        protected void MuestraEditarDetalle(DetalleFacturaTipo elDetalle)
        {
            try
            {
                //muestra la ventana
                frmAddDetalles.Visible = true;
                frmAddDetalles.Hidden = false;
                
                //setea los datos almacenados
                txtID_DetalleFacturaTipo.Text = elDetalle.ID_DetalleFacturaTipo.ToString();
                txtIDFacturaTipo.Text = elDetalle.ID_FacturaTipo.ToString();
                cmbCadenaComercial.SetValue(elDetalle.ID_CadenaComercial);
                cmbEventos.SetValue(elDetalle.ID_Evento);
                cmbTipoCuenta.SetValue(elDetalle.ID_TipoCuenta);
                txtformulaCantidad.Text=elDetalle.FormulaCantidad;
                txtformulaPrecioUnitario.Text=elDetalle.FormulaPrecioUnitario;
                txtformulaTotalDetalle.Text = elDetalle.FormulaTotal;

                
            }
            catch (Exception)
            {
            }
        }


        protected void MuestraEditarFacturaTipo(FacturaTipo laFactura)
        {
            try
            {
                //muestra la ventana


                //setea los datos almacenados
                cmbTipoColectivaEmisor.SetValueAndFireSelect(laFactura.ID_TipoColectivaEM);
                cmbtipoColectivaReceptor.SetValueAndFireSelect(laFactura.ID_TipoColectivaRE);


                DataSet dsEmisores = DAOCatalogos.ListaTiposColectivasPorID_TipoColectivaFacturacion(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), laFactura.ID_TipoColectivaEM);

                List<ColectivaComboPredictivo> ListaEmisores = new List<ColectivaComboPredictivo>();

                foreach (DataRow emisor in dsEmisores.Tables[0].Rows)
                {
                    var ComboEmisores = new ColectivaComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt64(emisor["ID_Colectiva"].ToString()),
                        ClaveColectiva = emisor["ClaveColectiva"].ToString(),
                        NombreORazonSocial = emisor["NombreORazonSocial"].ToString(),
                        NameFin = emisor["NameFin"].ToString()
                    };
                    ListaEmisores.Add(ComboEmisores);
                }

                StoreEmisor.DataSource = ListaEmisores;
                StoreEmisor.DataBind();
                

                DataSet dsReceptores = DAOCatalogos.ListaTiposColectivasPorID_TipoColectivaFacturacion(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), laFactura.ID_TipoColectivaRE);

                List<ColectivaComboPredictivo> ListaReceptores = new List<ColectivaComboPredictivo>();

                foreach (DataRow receptor in dsReceptores.Tables[0].Rows)
                {
                    var ComboReceptores = new ColectivaComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt64(receptor["ID_Colectiva"].ToString()),
                        ClaveColectiva = receptor["ClaveColectiva"].ToString(),
                        NombreORazonSocial = receptor["NombreORazonSocial"].ToString(),
                        NameFin = receptor["NameFin"].ToString()
                    };
                    ListaReceptores.Add(ComboReceptores);
                }

                StoreReceptor.DataSource = ListaReceptores;
                StoreReceptor.DataBind();

                cmbColectivaEmisor.SetValue(laFactura.ID_ColectivaEmisora);
                cmbColectivaReceptor.SetValue(laFactura.ID_ColectiraReceptora);

                cmbNivelDatos.SetValue(laFactura.ID_TipoColectivaNivelDatos);
                cmbNivelReceptor.SetValue(laFactura.ID_TipoColectivaReceptor);

                CmbPeriodo.SetValue(laFactura.ID_Periodo);

                cmbTipoFactura.SetValue(laFactura.ID_TipoDocumento);
                
                txtNombreFacturaTipo.Text = laFactura.Descripcion;
                txtIDFac.Text = laFactura.ID_FacturaTipo.ToString();

                cmbFolio.SelectedItem.Value = laFactura.Foliada ? "true" : "false";
                cmbFolio.SelectedItem.Text = laFactura.Foliada ? "SI" : "NO";

            }
            catch (Exception)
            {
            }
        }

        //[DirectMethod(Namespace = "AddFacturaTipo")]
        //public void StopMask()
        //{
        //    Thread.Sleep(5000);
        //    X.Mask.Hide();
        //}
    }
}