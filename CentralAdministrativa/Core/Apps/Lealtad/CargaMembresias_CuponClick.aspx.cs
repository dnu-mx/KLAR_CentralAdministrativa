using ClosedXML.Excel;
using DALCentralAplicaciones.Utilidades;
using DALLealtad.BaseDatos;
using DALLealtad.LogicaNegocio;
using Excel;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace Lealtad
{
    public partial class CargaMembresias_CuponClick : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    dfFechaInicio.MaxDate = DateTime.Today;
                    dfFechaInicio.SetValue(DateTime.Today);
                    
                    dfFechaFin.MaxDate = DateTime.Today;
                    dfFechaFin.SetValue(DateTime.Today);
                    
                    PagingToolBar1.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtMembresias", null);
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
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            this.txtMembresia.Clear();
            this.dfFechaInicio.SetValue(DateTime.Today);
            this.dfFechaFin.SetValue(DateTime.Today);

            LimpiaGridMembresias();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al Grid de folios
        /// </summary>
        protected void LimpiaGridMembresias()
        {
            this.btnExportExcel.Disabled = true;
            HttpContext.Current.Session.Add("DtMembresias", null);
            StoreMembresias.RemoveAll();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de folios, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelMembresias(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;

            if ((dfFechaInicio.SelectedDate != DateTime.MinValue && dfFechaFin.SelectedDate == DateTime.MinValue) ||
                dfFechaInicio.SelectedDate == DateTime.MinValue && dfFechaFin.SelectedDate != DateTime.MinValue)
            {
                X.Mask.Hide();
                X.Msg.Alert("Membresías", "Debes completar los filtros de fecha inicio y fecha fin").Show();
                return;
            }

            if (txtMembresia.Text == "" && 
                dfFechaInicio.SelectedDate == DateTime.MinValue &&
                dfFechaFin.SelectedDate == DateTime.MinValue)
            {
                X.Mask.Hide();
                X.Msg.Alert("Membresías", "Al menos debes llenar el filtro de Membresia").Show();
                return;
            }

            

            try
            {
                DataTable dtMembresias = new DataTable();

                dtMembresias = HttpContext.Current.Session["DtMembresias"] as DataTable;

                if (dtMembresias == null)
                {
                    dtMembresias = DAOEcommerceCuponClick.ObtieneReporteMembresiasCuponClick(
                        this.txtMembresia.Text, 
                        Convert.ToDateTime(this.dfFechaInicio.SelectedDate),
                        Convert.ToDateTime(this.dfFechaFin.SelectedDate),
                        this.Usuario);

                    HttpContext.Current.Session.Add("DtMembresias", dtMembresias);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtMembresias.Rows.Count < 1)
                {
                    X.Msg.Alert("Membresías", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtMembresias.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Membresías", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "Lealtad.ClicDePaso()",
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
                    int TotalRegistros = dtMembresias.Rows.Count;

                    (this.StoreMembresias.Proxy[0] as PageProxy).Total = TotalRegistros;
                    
                    DataTable sortedDT = dtMembresias.Clone();
                    DataTable dtToGrid = dtMembresias.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtMembresias.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingToolBar1.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtMembresias.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreMembresias.DataSource = dtToGrid;
                    StoreMembresias.DataBind();

                    this.btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Membresías", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Membresías", "Ocurrió un Error al Consultar el Reporte de Membresías").Show();
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
            LimpiaGridMembresias();

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
        /// Controla el evento onRefresh del grid de folios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreMembresias_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelMembresias(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "Lealtad")]
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
        [DirectMethod(Namespace = "Lealtad")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteMembresias";
                DataTable _dtMembresias = HttpContext.Current.Session["DtMembresias"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtMembresias, reportName);

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
                X.Msg.Alert("Membresías", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
            }

            return ws;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "Lealtad")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }


        public void btnCargarArchivo_Click(object sender, DirectEventArgs e)
        {
            try
            {
                //   Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
                HttpPostedFile file = this.FileUploadField1.PostedFile;

                string[] directorios = file.FileName.Split('\\');
                string fileName = directorios[directorios.Count() - 1];

                //Se valida que se haya seleccionado un archivo
                if (String.IsNullOrEmpty(fileName))
                {
                    X.Msg.Alert("Cargar Archivo", "Selecciona un archivo para cargarlo.").Show();
                    return;
                }

                //Se valida que se haya seleccionado un archivo Excel
                if (!fileName.Contains(".xlsx"))
                {
                    X.Msg.Alert("Cargar Archivo", "El archivo seleccionado no es del formato Excel soportado (*.xlsx). Verifica tu archivo.").Show();
                    return;
                }

                string tempPath = "C:\\TmpXlsxFiles\\";

                //Si no existe el directorio, lo crea
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                tempPath += fileName;

                //Se almacena archivo en directorio temporal del que es dueño el aplicativo
                //para no tener problemas de permisos de apertura
                file.SaveAs(tempPath);

                FileStream stream = File.Open(tempPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = null;

                //Reading from a OpenXml Excel file (2007 format; *.xlsx)
                if (fileName.Contains(".xlsx"))
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    //DataSet - Create column names from first row
                    excelReader.IsFirstRowAsColumnNames = true;
                }

                if (!String.IsNullOrEmpty(excelReader.ExceptionMessage))
                {
                    throw new Exception("IExcelDataReader exception: " + excelReader.ExceptionMessage);
                }

                DataSet dsArchivo = excelReader.AsDataSet();


                /* */
                DataTable dtCloned = dsArchivo.Tables[0].Clone();
                DataTable dtClonedShow = dsArchivo.Tables[0].Clone();//tabla a mostrar

                int NumColumnas = dsArchivo.Tables[0].Columns.Count;
                int NumColumnasLayout = 3; 

                if (NumColumnas > NumColumnasLayout)
                {
                    for (int iCounter = NumColumnasLayout; iCounter < NumColumnas; iCounter++)
                    {
                        dtCloned.Columns.RemoveAt(iCounter);
                        dtClonedShow.Columns.RemoveAt(iCounter);
                    }
                }

                NumColumnas = dtCloned.Columns.Count;

                int val = 0;

                for (int i = 0; i < NumColumnas; i++)
                {
                    dtCloned.Columns[i].DataType = typeof(string);
                    dtClonedShow.Columns[i].ColumnName = dtCloned.Columns[i].ColumnName.ToUpper();
                    dtClonedShow.Columns[i].DataType = typeof(string);
                }

                foreach (DataRow row in dsArchivo.Tables[0].Rows)
                {
                    for (int i = 0; i < NumColumnas; i++)
                    {
                        if (string.IsNullOrEmpty(row.ItemArray[i].ToString()))
                        {
                            dtCloned.Columns[i].DataType = typeof(string);
                        }
                    }
                    val++;
                }


                foreach (DataRow row in dsArchivo.Tables[0].Rows)
                {
                    //Se eliminan espacios en blanco de más, 
                    //al inicio o al final, de todos los datos
                    for (int column = 0; column < NumColumnasLayout; column++)
                    {
                        row[column] = row[column].ToString().Trim();
                    }

                    dtCloned.ImportRow(row);
                    dtClonedShow.ImportRow(row);
                }

                dtCloned.Columns[1].ColumnName = "Membresia";

                dtClonedShow.Columns[0].ColumnName = "IDCliente";
                dtClonedShow.Columns[1].ColumnName = "Membresia";
                dtClonedShow.Columns[2].ColumnName = "Vigencia";

                //Se verifica la consistencia de información en las cadenas
                if (VerificaDatosCadenas(dtCloned))
                {
                    return;
                }

                //Se valida la longitud de los campos de tipo cadena
                if (VerificaLongitudCampos(dtCloned))
                {
                    return;
                }

                //Se verifica contenido y formato de cada columna
                if (VerificaFormatoDatosArchivo(dtCloned, NumColumnas))
                {
                    return;
                }


                //CASO ESPECIAL: Se da formato a los campos de fecha para mostrarlos en el grid
                EstableceFormatoFechas(dtClonedShow);

                //El archivo pasó todas las validaciones
                dtCloned.AcceptChanges();

                LNEcommerceCuponClick.InsertaMembresiasTMP(dtCloned, this.Usuario);
                X.Msg.Notify("Cargar Archivo", "Archivo cargado correctamente.<br/>Selecciona Aplicar Cambios").Show();

                GridDatosArchivo.GetStore().DataSource = dtClonedShow;
                GridDatosArchivo.GetStore().DataBind();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Membresías", ex.Message).Show();
            }
        }

        /// <summary>
        /// Valida que la longitud de los campos de tipo varchar no exceda el tamaño máximo
        /// del campo correspondiente en base de datos
        /// </summary>
        /// <param name="_dt">DataTable con la información del archivo Excel</param>
        /// <returns>TRUE si se excede la longitud en algún dato</returns>
        protected bool VerificaLongitudCampos(DataTable _dt)
        {
            bool validacion = false;
            int Membresia = 13;
            int rowIndex = 2;

            try
            {
                foreach (DataRow row in _dt.Rows)
                {
                    if ((row["IDCliente"].ToString().Length + row["Membresia"].ToString().Length) != Membresia)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud del <b>IDCliente</b> + <b>Membresia</b> debe ser de " + Membresia.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    
                    rowIndex++;
                }
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Membresías", ex.Message).Show();
            }

            return validacion;
        }

        /// <summary>
        /// Verifica que los datos de la cadena (clave, nombre y giro) sean siempre los mismos, 
        /// sin importar el número de promociones que se deseen añadir.
        /// </summary>
        /// <param name="_dtClonado">DataTable con la información del archivo Excel</param>
        /// <returns>TRUE si encuentran datos inconsistentes</returns>
        protected bool VerificaDatosCadenas(DataTable _dtClonado)
        {
            Boolean resultado = false;

            var clavesDuplicadas = _dtClonado.AsEnumerable()
               .Select(dr => dr.Field<string>("IDCLIENTE") + "-" + dr.Field<string>("MEMBRESIA"))
               .GroupBy(x => x)
               .Where(g => g.Count() > 1)
               .Select(h => h.Key)
               .ToList();

            if (clavesDuplicadas.Count > 0)
            {
                X.Msg.Alert("Error de contenido en archivo excel",
                            "El IDCliente y la Membresía no pueden repetirse [" + clavesDuplicadas[0].ToString() + "]").Show();
                resultado = true;
            }

            return resultado;
        }

        /// <summary>
        /// Establece formato yyyy/MM/dd HH:mm:ss a los campos de fecha del DataTable
        /// que se mostrará en el grid
        /// </summary>
        /// <param name="_dtGrid">Objeto con los datos a mostrar en el grid</param>
        protected void EstableceFormatoFechas(DataTable _dtGrid)
        {
            string cadenaorig, subcadena;
            string[] partes;

            foreach (DataRow row in _dtGrid.Rows)
            {
                foreach (DataColumn dc in _dtGrid.Columns)
                {
                    if (dc.ColumnName.Contains("Vigencia")) // || dc.ColumnName.Contains("FECHAFIN"))
                    {
                        cadenaorig = row[dc].ToString();
                        partes = cadenaorig.Split(' ');
                        subcadena = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(partes[0]));

                        DateTime fecha = DateTime.ParseExact(subcadena, "dd/MM/yyyy",
                            CultureInfo.GetCultureInfo("es-MX").DateTimeFormat);
                        row[dc] = fecha.ToString("yyyy/MM/dd HH:mm:ss");
                    }
                }
            }
        }

        /// <summary>
        /// Verifica el formato de los datos que corresponden a cada columna, así como aquellas que no deberían tener
        /// celdas en blanco. Si no se cumplen las validaciones, se muestra mensaje de error con la fila y columna
        /// en donde está el error.
        /// </summary>
        /// <param name="_dtFile">Datatable con las columnas del archivo</param>
        /// <param name="totalColumnas">Número total de columnas del archivo</param>
        /// <returns>TRUE en caso de error</returns>
        protected bool VerificaFormatoDatosArchivo(DataTable _dtFile, int totalColumnas)
        {
            int fila = 0;
            int columna = 0;
            bool error = false;
            int errorBlancos = 0;

            foreach (DataRow row in _dtFile.Rows)
            {
                string letra = ((char)((int)'A')).ToString();

                for (int i = 0; i < totalColumnas; i++)
                {
                    if (i == 2) 
                    {
                        try
                        {
                            string cadenaorig = row[i].ToString();
                            string[] partes = cadenaorig.Split(' ');
                            string subcadena = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(partes[0]));

                            DateTime fecha = DateTime.ParseExact(subcadena, "dd/MM/yyyy", CultureInfo.GetCultureInfo("es-MX").DateTimeFormat);
                            row[i] = fecha.ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        catch (Exception)
                        {
                            X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                + ")</b>, debe ser una fecha válida en formato(dd/mm/aaaa)").Show();
                            error = true;
                            break;
                        }
                    }

                    columna++;
                    letra = ((char)((int)letra[0] + 1)).ToString();
                }

                if (error == true)
                {
                    break;
                }
                else
                {
                    fila++;
                }

                if (errorBlancos == 1)
                {
                    for (int i = fila - 1; i < _dtFile.Rows.Count; i++)
                    {
                        DataRow dr = _dtFile.Rows[i];
                        dr.Delete();
                    }
                    break;
                }
            }

            return error;
        }

        /// <summary>
        /// Realiza la validación de los datos en la tabla, los almacena en la tabla
        /// temporal de base de datos y los muestra el el grid correspondiente.
        /// </summary>
        /// <param name="ds">DataSet con los datos del archivo Excel</param>
        protected void ValidaMuestraYSubeArchivo(DataSet ds)
        {
            Int32 rowIndex = 0, finalRow;

            //Se establece la última fila con datos en la tabla
            finalRow = ObtieneFilaFin(ds);

            //Se realizan dos clones de la tabla para manipularla
            //una para poder exportarla directamente a BD
            DataTable dt = ds.Tables[0].Clone();
            //y otra para mostrarla en el grid
            DataTable dt_ToScreen = ds.Tables[0].Clone();

            //Se establece el tipo de datos de cada columna en la tabla clonada
            //(obligatorio para la exportación a BD)
            for (int columna = 0; columna < ds.Tables[0].Columns.Count; columna++)
            {
                dt.Columns[columna].DataType = typeof(string);
                dt_ToScreen.Columns[columna].DataType = typeof(string);
            }

            dt_ToScreen.Columns.Add("ID", typeof(string));

            //Se importan las filas de la tabla original a la clonada
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                dt.ImportRow(row);
                dt_ToScreen.ImportRow(row);
                dt_ToScreen.Rows[rowIndex]["ID"] = rowIndex + 1;
                rowIndex++;
            }

            NombreColumnas(dt);
            NombreColumnas(dt_ToScreen);
            rowIndex = 0;

            //Validación de datos en las celdas
            foreach (DataRow row in dt.Rows)
            {
                if (rowIndex > finalRow)
                    break;

                if (String.IsNullOrEmpty(row.ItemArray[0].ToString()))
                {
                    X.Msg.Alert("Actualizar Membresías",
                        "El ID Cliente no puede estar vacío. <b>(Fila no. " +
                        (rowIndex + 2).ToString() + ")</b>").Show();
                    return;
                }

                if (String.IsNullOrEmpty(row.ItemArray[1].ToString()))
                {
                    X.Msg.Alert("Actualizar Membresías",
                        "El Número de Membresía no puede estar vacío. <b>(Fila no. " +
                        (rowIndex + 2).ToString() + ")</b>").Show();
                    return;
                }

                if (String.IsNullOrEmpty(row.ItemArray[2].ToString()))
                {
                    X.Msg.Alert("Actualizar Membresías",
                        "La Vigencia no puede estar vacía. <b>(Fila no. " +
                        (rowIndex + 2).ToString() + ")</b>").Show();
                    return;
                }

                rowIndex++;
            }

            //Se eliminan las filas extras de la tabla que añade la clonación 
            for (int fila = finalRow + 1; fila <= dt.Rows.Count - 1; fila++)
            {
                DataRow dr = dt.Rows[fila];
                dr.Delete();
            }

            dt.AcceptChanges();
            LNEcommerceCuponClick.InsertaMembresiasTMP(dt, this.Usuario);

            GridDatosArchivo.GetStore().DataSource = dt_ToScreen;
            GridDatosArchivo.GetStore().DataBind();
        }

        /// <summary>
        /// Obtiene el número de fila donde termina la información de la tabla
        /// </summary>
        /// <param name="ds">DataSet con los datos</param>
        /// <returns>Número final de fila</returns>
        protected int ObtieneFilaFin(DataSet ds)
        {
            int registro;
            string dummy = "";

            for (registro = 0; registro < ds.Tables[0].Rows.Count; registro++)
            {
                try
                {
                    dummy = (String.IsNullOrEmpty(ds.Tables[0].Rows[registro + 1][0].ToString().Trim()) &&
                        String.IsNullOrEmpty(ds.Tables[0].Rows[registro + 1][1].ToString().Trim()) &&
                        String.IsNullOrEmpty(ds.Tables[0].Rows[registro + 1][2].ToString().Trim())) ? "" : "A";

                    if (String.IsNullOrEmpty(dummy))
                        break;
                }
                catch (Exception)
                {
                    break;
                }
            }

            return registro;
        }

        /// <summary>
        /// Establece el nombre apropiado para cada columna, según la configuración
        /// del Grid
        /// </summary>
        /// <param name="dt_">DataTable del que se establecen los nombres de columnas</param>
        protected void NombreColumnas(DataTable dt_)
        {
            dt_.Columns[0].ColumnName = "IDCliente";
            dt_.Columns[1].ColumnName = "Membresia";
            dt_.Columns[2].ColumnName = "Vigencia";
        }

        /// <summary>
        /// Controla el evento Click al botón Aplicar Cambios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directo del evento que se ejecutó</param>
        public void btnAplicarCambios_Click(object sender, DirectEventArgs e)
        {
            try
            {
                int programa = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Programa_Membresias_CuponClick").Valor);

                string respuestaBD = LNEcommerceCuponClick.AplicaCambiosAMembresias(programa, this.Usuario);

                if (respuestaBD.ToUpper().Contains("ERROR"))
                {
                    X.Msg.Alert("Actualizar Membresías", respuestaBD + "<br /> ").Show();
                }
                else
                {
                    X.Msg.Notify("Actualizar Membresías", respuestaBD + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                }
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Membresías", "Ocurrió un error al aplicar los cambios en base de datos.").Show();
            }
        }
    }
}