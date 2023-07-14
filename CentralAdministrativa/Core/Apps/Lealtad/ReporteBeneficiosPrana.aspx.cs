using ClosedXML.Excel;
using DALCentralAplicaciones.Utilidades;
using DALLealtad.BaseDatos;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace Lealtad
{
    public partial class ReporteBeneficiosPrana : DALCentralAplicaciones.PaginaBaseCAPP
    {
        const int COL_FECHA_ALTA = 44;
        const int COL_FECHA_MODIFICA = 46;
        const int COL_FECHA_BAJA = 48;

        const int COL_PROGRAMA_INICIO = 19;
        const int COL_PROGRAMA_ULTIMO = 42;

        public class CadenaComboPredictivo
        {
            public Int64 ID_Cadena { get; set; }
            public String ClaveCadena { get; set; }
            public String NombreComercial { get; set; }
        }


        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Beneficios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DataSet dsCadenas = DAOEcommercePrana.ListaCadenas(this.Usuario);

                    List<CadenaComboPredictivo> ComboList = new List<CadenaComboPredictivo>();

                    foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
                    {
                        var cadenaCombo = new CadenaComboPredictivo()
                        {
                            ID_Cadena = Convert.ToInt32(cadena["ID_Cadena"].ToString()),
                            ClaveCadena = cadena["ClaveCadena"].ToString(),
                            NombreComercial = cadena["NombreComercial"].ToString()
                        };
                        ComboList.Add(cadenaCombo);
                    }

                    StoreClaveCadena.DataSource = ComboList;
                    StoreClaveCadena.DataBind();

                    StoreGiros.DataSource = DAOEcommercePrana.ListaGiros(this.Usuario);
                    StoreGiros.DataBind();

                    EstableceProgramas();

                    dfFechaInicio.MaxDate = DateTime.Today;
                    dfFechaFin.MaxDate = DateTime.Today;

                    PagingToolBar1.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtBeneficios", null);
                }

                if (!X.IsAjaxRequest)
                {

                }
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Establece los programas que el usuario tiene permitidos para la aplicación
        /// en el combo correspondiente
        /// </summary>
        protected void EstableceProgramas()
        {
            try
            {
                DataSet dsProgramas = DAOEcommercePrana.ListaProgramas(this.Usuario,
                           Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                StoreProgramas.DataSource = dsProgramas;
                StoreProgramas.DataBind();

                cBoxPrograma.SetValue(dsProgramas.Tables[0].Rows[0]["ID_Programa"]);
                cBoxPrograma.SelectedItem.Text = dsProgramas.Tables[0].Rows[0]["Descripcion"].ToString();

                //Se añaden sólo las columnas de los programas permitidos
                foreach (DataRow dr in dsProgramas.Tables[0].Rows)
                {
                    if (Convert.ToInt16(dr["ID_Programa"]) > -1)
                    {
                        Column col = new Column();
                        col.DataIndex = dr["Clave"].ToString();
                        col.Header = dr["Clave"].ToString();
                        col.Sortable = true;

                        GridPanelBeneficios.ColumnModel.Columns.Add(col);
                    }
                }

                AgregaColumnasFijas();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, "");
                throw ex;
            }
        }

        /// <summary>
        /// Añade al grid las columnas fijas que van después de los programas
        /// </summary>
        protected void AgregaColumnasFijas()
        {
            Column col1 = new Column();
            col1.DataIndex = "UsuarioAlta";
            col1.Header = "UsuarioAlta";
            col1.Sortable = true;

            DateColumn col2 = new DateColumn();
            col2.DataIndex = "FechaAlta";
            col2.Header = "FechaAlta";
            col2.Sortable = true;
            col2.Format = "dd/MM/yyyy HH:mm:ss";

            Column col3 = new Column();
            col3.DataIndex = "UsuarioModifico";
            col3.Header = "UsuarioModifico";
            col3.Sortable = true;

            DateColumn col4 = new DateColumn();
            col4.DataIndex = "FechaModifico";
            col4.Header = "FechaModifico";
            col4.Sortable = true;
            col4.Format = "dd/MM/yyyy HH:mm:ss";

            Column col5 = new Column();
            col5.DataIndex = "UsuarioBaja";
            col5.Header = "UsuarioBaja";
            col5.Sortable = true;

            DateColumn col6 = new DateColumn();
            col6.DataIndex = "FechaBaja";
            col6.Header = "FechaBaja";
            col6.Sortable = true;
            col6.Format = "dd/MM/yyyy HH:mm:ss";

            Column col7 = new Column();
            col7.DataIndex = "Presencia";
            col7.Header = "Presencia";
            col7.Sortable = true;

            Column col8 = new Column();
            col8.DataIndex = "Clasificacion";
            col8.Header = "Clasificación";
            col8.Sortable = true;
            col8.Width = 200;

            Column col9 = new Column();
            col9.DataIndex = "PalabrasClave";
            col9.Header = "Palabras Clave";
            col9.Sortable = true;
            col9.Width = 500;

            Column col10 = new Column();
            col10.DataIndex = "CuentaCLABE";
            col10.Header = "Cuenta CLABE";
            col10.Sortable = true;
            col10.Width = 150;

            Column col11 = new Column();
            col11.DataIndex = "NombreContacto";
            col11.Header = "Nombre Contacto";
            col11.Sortable = true;
            col11.Width = 200;

            Column col12 = new Column();
            col12.DataIndex = "TelefonoContacto";
            col12.Header = "Tel. Contacto";
            col12.Sortable = true;
            col12.Width = 100;

            Column col13 = new Column();
            col13.DataIndex = "Cargo";
            col13.Header = "Cargo";
            col13.Sortable = true;
            col13.Width = 100;

            Column col14 = new Column();
            col14.DataIndex = "CelularContacto";
            col14.Header = "Cel. Contacto";
            col14.Sortable = true;
            col14.Width = 100;

            Column col15 = new Column();
            col15.DataIndex = "Correo";
            col15.Header = "Correo";
            col15.Sortable = true;
            col15.Width = 200;

            Column col16 = new Column();
            col16.DataIndex = "Extracto";
            col16.Header = "Extracto";
            col16.Sortable = true;
            col16.Width = 200;

            Column col17 = new Column();
            col17.DataIndex = "SubGiro";
            col17.Header = "SubGiro";
            col17.Sortable = true;
            col17.Width = 200;

            Column col18 = new Column();
            col18.DataIndex = "TicketPromedio";
            col18.Header = "Ticket Promedio";
            col18.Sortable = true;
            col18.Width = 200;

            Column col19 = new Column();
            col19.DataIndex = "PerfilNSE";
            col19.Header = "PerfilNSE";
            col19.Sortable = true;
            col19.Width = 200;

            Column col20 = new Column();
            col20.DataIndex = "TipoRedencion";
            col20.Header = "Tipo Redención";
            col20.Sortable = true;
            col20.Width = 200;

            Column col21 = new Column();
            col21.DataIndex = "URLCupon";
            col21.Header = "URL Cupón";
            col21.Sortable = true;
            col21.Width = 200;

            Column col22 = new Column();
            col22.DataIndex = "Genero";
            col22.Header = "Género";
            col22.Sortable = true;
            col22.Width = 200;

            Column col23 = new Column();
            col23.DataIndex = "PromoPlus";
            col23.Header = "Promo Plus";
            col23.Sortable = true;
            col23.Width = 200;

            Column col24 = new Column();
            col24.DataIndex = "RangoEdad";
            col24.Header = "Rango Edad";
            col24.Sortable = true;
            col24.Width = 200;

            Column col25 = new Column();
            col25.DataIndex = "TipoEstablecimiento";
            col25.Header = "Tipo de Establecimiento";
            col25.Sortable = true;
            col25.Width = 200;

            GridPanelBeneficios.ColumnModel.Columns.Add(col1);
            GridPanelBeneficios.ColumnModel.Columns.Add(col2);
            GridPanelBeneficios.ColumnModel.Columns.Add(col3);
            GridPanelBeneficios.ColumnModel.Columns.Add(col4);
            GridPanelBeneficios.ColumnModel.Columns.Add(col5);
            GridPanelBeneficios.ColumnModel.Columns.Add(col6);
            GridPanelBeneficios.ColumnModel.Columns.Add(col7);
            GridPanelBeneficios.ColumnModel.Columns.Add(col8);
            GridPanelBeneficios.ColumnModel.Columns.Add(col9);      
            GridPanelBeneficios.ColumnModel.Columns.Add(col10);
            GridPanelBeneficios.ColumnModel.Columns.Add(col11);
            GridPanelBeneficios.ColumnModel.Columns.Add(col12);
            GridPanelBeneficios.ColumnModel.Columns.Add(col13);
            GridPanelBeneficios.ColumnModel.Columns.Add(col14);
            GridPanelBeneficios.ColumnModel.Columns.Add(col15);
            GridPanelBeneficios.ColumnModel.Columns.Add(col16);

            GridPanelBeneficios.ColumnModel.Columns.Add(col17);
            GridPanelBeneficios.ColumnModel.Columns.Add(col18);
            GridPanelBeneficios.ColumnModel.Columns.Add(col19);
            GridPanelBeneficios.ColumnModel.Columns.Add(col20);
            GridPanelBeneficios.ColumnModel.Columns.Add(col21);
            GridPanelBeneficios.ColumnModel.Columns.Add(col22);
            GridPanelBeneficios.ColumnModel.Columns.Add(col23);
            GridPanelBeneficios.ColumnModel.Columns.Add(col24);
            GridPanelBeneficios.ColumnModel.Columns.Add(col25);
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            cBoxPrograma.Reset();
            cBoxClaveCadena.Clear();
            txtCadena.Clear();
            cBoxGiro.Clear();
            cBoxActiva.Clear();
            dfFechaInicio.Clear();
            dfFechaFin.Clear();

            LimpiaGridBeneficios();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al Grid de Beneficios
        /// </summary>
        protected void LimpiaGridBeneficios()
        {
            this.btnExportExcel.Disabled = true;
            this.btnExportCSV.Disabled = true;

            HttpContext.Current.Session.Add("DtBeneficios", null);
            StoreBeneficios.RemoveAll();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de beneficios, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelBeneficios(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;
            this.btnExportCSV.Disabled = true;
            
            try
            {
                DataTable dtBeneficios = new DataTable();

                dtBeneficios = HttpContext.Current.Session["DtBeneficios"] as DataTable;

                if (dtBeneficios == null)
                {

                    int id_cadena = String.IsNullOrEmpty(this.cBoxClaveCadena.SelectedItem.Value) ?
                        -1 : int.Parse(this.cBoxClaveCadena.SelectedItem.Value);

                    dtBeneficios = DAOEcommercePrana.ObtieneReportePromociones(
                        String.IsNullOrEmpty(this.cBoxPrograma.SelectedItem.Value) ?
                        -1 : int.Parse(this.cBoxPrograma.SelectedItem.Value),
                        id_cadena, this.txtCadena.Text,
                        String.IsNullOrEmpty(this.cBoxGiro.SelectedItem.Value) ?
                        -1 : int.Parse(this.cBoxGiro.SelectedItem.Value),
                        String.IsNullOrEmpty(this.cBoxActiva.SelectedItem.Value) ?
                        -1 : int.Parse(this.cBoxActiva.SelectedItem.Value),
                        Convert.ToDateTime(this.dfFechaInicio.SelectedDate),
                        Convert.ToDateTime(this.dfFechaFin.SelectedDate),
                        this.Usuario);

                    HttpContext.Current.Session.Add("DtBeneficios", dtBeneficios);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtBeneficios.Rows.Count < 1)
                {
                    X.Msg.Alert("Beneficios", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtBeneficios.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Beneficios", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepBenefPrana.ClicDePaso()",
                                Text = "Aceptar"
                            },
                            No = new MessageBoxButtonConfig
                            {
                                Text = "Cancelar"
                            }
                        }).Show();
                }
                else
                {
                    int TotalRegistros = dtBeneficios.Rows.Count;

                    (this.StoreBeneficios.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtBeneficios.Clone();
                    DataTable dtToGrid = dtBeneficios.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtBeneficios.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingToolBar1.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtBeneficios.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreBeneficios.DataSource = dtToGrid;
                    StoreBeneficios.DataBind();

                    this.btnExportExcel.Disabled = false;
                    this.btnExportCSV.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Beneficios", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Beneficios", "Ocurrió un Error al Consultar el Reporte de Beneficios").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, DirectEventArgs e)
        {
            LimpiaGridBeneficios();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento SUBMIT al querer exportar al formato seleccionado
        /// los resultados de la consulta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void StoreSubmit(object sender, StoreSubmitDataEventArgs e)
        {
            string format = this.FormatType.Value.ToString();

            XmlNode xml = e.Xml;

            this.Response.Clear();

            switch (format)
            {
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
        /// Controla el evento onRefresh del grid de beneficios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreBeneficios_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelBeneficios(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepBenefPrana")]
        public void ClicDePaso()
        {
            btnDownloadHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void Download(object sender, DirectEventArgs e)
        {
            ExportDataTableToExcel();
        }

        /// <summary>
        /// Exporta el reporte previamento consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepBenefPrana")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteBeneficios";
                DataTable _dtBeneficios = HttpContext.Current.Session["DtBeneficios"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtBeneficios, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsColumns(ws, ws.Column(1).CellsUsed().Count());

                //Se prepara la respuesta
                this.Response.Clear();
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.Buffer = false;

                this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=" + reportName + ".xlsx");

                //Se envía el reporte como respuesta
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(this.Response.OutputStream);
                    memoryStream.Close();
                }

                this.Response.End();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Beneficios", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
            }
        }

        /// <summary>
        /// Establece el formato deseado a las columnas de la hoja de trabajo por exportar
        /// </summary>
        /// <param name="ws">Hoja de trabajo</param>
        /// <param name="rowsNum">Total de filas de la hoja de trabajo</param>
        /// <returns></returns>
        protected IXLWorksheet FormatWsColumns(IXLWorksheet ws, int rowsNum)
        {
            ws.Column(1).Hide();

            //Columnas de fecha y hora (alta, modificación y cambio)
            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, COL_FECHA_ALTA).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, COL_FECHA_MODIFICA).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, COL_FECHA_BAJA).SetDataType(XLCellValues.DateTime);
            }

            //Consideración especial para ocultar de reporte los programas que el usuario
            //no tenga permitidos y/o que no haya seleccionado como filtro
            if (int.Parse(cBoxPrograma.SelectedItem.Value) != -1)
            {
                for (int i = COL_PROGRAMA_INICIO; i <= COL_PROGRAMA_ULTIMO; i++)
                {
                    for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                    {
                        if (ws.Cell(1, i).GetString() != cBoxPrograma.SelectedItem.Text)
                        {
                            ws.Cell(rowsCounter, i).Value = "";
                        }
                    }
                }

                for (int i = COL_PROGRAMA_INICIO; i <= COL_PROGRAMA_ULTIMO; i++)
                {
                    if (ws.Cell(1, i).GetString() != cBoxPrograma.SelectedItem.Text)
                    {
                        ws.Column(i).Hide();
                    }
                }
            }

            return ws;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepBenefPrana")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}