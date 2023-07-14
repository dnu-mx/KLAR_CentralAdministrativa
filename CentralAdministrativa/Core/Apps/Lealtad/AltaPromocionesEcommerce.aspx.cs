using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALLealtad.BaseDatos;
using DALLealtad.LogicaNegocio;
using DALLealtad.Utilidades;
using Excel;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml;

namespace Lealtad
{
    public partial class AltaPromocionesEcommerce : PaginaBaseCAPP
    {
        #region Variables privadas

        /// <summary>
        /// Expresión regular de validación para email
        /// </summary>
        private const string regexEmail = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+${7,100}";

        #endregion


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
                    HttpContext.Current.Session.Add("DtSucursalesPromo", null);
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Consulta a base de datos las cadenas y llena el grid correspondiente
        /// </summary>
        protected void LlenaGridPanelCadenas()
        {
            try
            {
                StoreCadenas.RemoveAll();

                DataSet dsCadenas = DAOEcommercePrana.ConsutaCadenasPromociones(
                    txtClaveCadena.Text, txtNombreCadena.Text, this.Usuario);

                if (dsCadenas.Tables[0].Rows.Count < 1)
                {
                    X.Msg.Alert("Consulta de Cadenas", "No existen coincidencias con la búsqueda solicitada.").Show();
                    return;
                }
                else
                {
                    StoreCadenas.DataSource = dsCadenas;
                    StoreCadenas.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consulta de Cadenas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Cadenas", "Ocurrió un Error al Buscar las Cadenas").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click en el botón de búsqueda de cadenas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnBuscarCadenas_Click(object sender, DirectEventArgs e)
        {
            LlenaGridPanelCadenas();

            this.StorePromociones.RemoveAll();

            this.StoreUsuarios.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            hdnIdCadena.Clear();
            hdnClaveCadena.Clear();
            txtClaveCadena.Clear();
            txtNombreCadena.Clear();
            StoreCadenas.RemoveAll();

            StorePromociones.RemoveAll();
            StoreUsuarios.RemoveAll();
            btnExportExcel.Disabled = true;
            PanelSur.Collapsed = true;

            LimpiaControlesPromo();
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de cadenas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectCadena_Event(object sender, DirectEventArgs e)
        {
            try
            {
                string json = e.ExtraParams["Values"];

                IDictionary<string, string>[] cadenaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (cadenaSeleccionada == null || cadenaSeleccionada.Length < 1)
                {
                    return;
                }

                int idCadena = 0;
                string claveCadena = "";

                foreach (KeyValuePair<string, string> columna in cadenaSeleccionada[0])
                {
                    switch (columna.Key)
                    {
                        case "ID_Cadena": idCadena = int.Parse(columna.Value); break;
                        case "ClaveCadena": claveCadena = columna.Value; break;
                        default:
                            break;
                    }
                }

                hdnIdCadena.Value = idCadena;
                hdnClaveCadena.Text = claveCadena;

                LlenaGridPromociones(idCadena);

                LlenaGridUsuarios(claveCadena);

                PanelSur.Collapsed = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cadena", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Cadena", "Ocurrió un Error al Seleccionar la Cadena").Show();
            }
        }

        /// <summary>
        /// Llena el grid de promociones con la información de base de datos e-Commerce
        /// </summary>
        /// <param name="idCadena">Identificador de la cadena seleccionada</param>
        protected void LlenaGridPromociones(int idCadena)
        {
            try
            {
                StorePromociones.RemoveAll();

                StorePromociones.DataSource = DAOEcommercePrana.ObtienePromocionesActivasCadena(idCadena, this.Usuario);
                StorePromociones.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Promociones", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Promociones", "Ocurrió un Error al Buscar las Promociones Activas").Show();
            }
        }

        /// <summary>
        /// Llena el grid de usuarios activos con la información de base de datos
        /// </summary>
        /// <param name="ClaveCadena">Clave de la cadena seleccionada</param>
        protected void LlenaGridUsuarios(String ClaveCadena)
        {
            try
            {
                StoreUsuarios.RemoveAll();
                StoreSucursales.RemoveAll();
                btnExportExcel.Disabled = true;

                DataSet dsUsuarios = DAOPromociones.ObtieneUsuariosActivosCadena(ClaveCadena, this.Usuario);

                if (dsUsuarios.Tables[0].Rows.Count > 0)
                {
                    btnExportExcel.Disabled = false;
                }

                StoreUsuarios.DataSource = dsUsuarios;
                StoreUsuarios.DataBind();

                StoreSucursales.DataSource = DAOEcommercePrana.ObtieneSucursalesActivasCadena(
                    Convert.ToInt32(hdnIdCadena.Value), this.Usuario);
                StoreSucursales.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Usuarios", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Usuarios", "Ocurrió un Error al Buscar los Usuarios Activos").Show();
            }
        }

        /// <summary>
        /// Limpia los controles y objetos relacionados al módulo de promociones
        /// </summary>
        protected void LimpiaControlesPromo()
        {
            HttpContext.Current.Session.Add("DtSucursalesPromo", null);
            RowSelectionModel _rsm = this.GridSucursales.GetSelectionModel() as RowSelectionModel;
            _rsm.SelectedRows.Clear();
            _rsm.UpdateSelection();

            chkBoxSams.Checked = false;
            chkBoxEdenred.Checked = false;

            chkBoxSams.Disabled = false;
            chkBoxEdenred.Disabled = false;

            StoreSucursalesPromo.RemoveAll();
        }

        /// <summary>
        /// Controla el evento de selección de comandos en el grid de promociones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void ModuloPromociones(object sender, DirectEventArgs e)
        {
            try
            {
                char[] charsToTrim = { '*', '"', '/' };
                String comando = e.ExtraParams["Comando"];
                int IdPromo = Convert.ToInt32(e.ExtraParams["ID_Promocion"]);
                hdnIdPromo.Value = IdPromo;
                string Promo = e.ExtraParams["Promocion"].Trim(charsToTrim);

                switch (comando)
                {
                    case "Configurar":
                    case "Editar":
                        LimpiaControlesPromo();

                        txaPromocion.Text = Promo;

                        DataSet dsProgramas = DAOEcommercePrana.ConsultaProgramasPromocion(IdPromo, this.Usuario);
                        chkBoxSams.Checked = dsProgramas.Tables[0].Rows[0]["Sams"].ToString() == "1" ? true : false;
                        chkBoxEdenred.Checked = dsProgramas.Tables[0].Rows[0]["Edenred"].ToString() == "1" ? true : false;

                        chkBoxSams.Disabled = chkBoxSams.Checked;
                        chkBoxEdenred.Disabled = chkBoxEdenred.Checked;
                        
                        DataTable dtSucPromo = DAOEcommercePrana.ConsultaSucursalesPromocion(IdPromo, this.Usuario);
                        StoreSucursalesPromo.DataSource = dtSucPromo;
                        StoreSucursalesPromo.DataBind();

                        HttpContext.Current.Session.Add("DtSucursalesPromo", dtSucPromo);

                        RowSelectionModel rsm = this.GridSucursales.GetSelectionModel() as RowSelectionModel;

                        foreach (DataRow row in dtSucPromo.Rows)
                        {
                            if (Convert.ToInt32(row["ActivaEnAutorizador"].ToString()) == 1)
                            {
                                rsm.SelectedRows.Add(new SelectedRow(dtSucPromo.Rows.IndexOf(row)));
                                rsm.UpdateSelection();
                            }
                        }

                        txaPromocion.Cls = Request.UserAgent.Contains("Edge") ? "wrap-area" : "";
                        WdwModuloPromos.Show();

                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla las llamadas a los objetos de datos responsables de la modificación de la promoción en
        /// el Autorizador
        /// </summary>
        [DirectMethod(Namespace = "AltaPromo", Timeout = 120000)]
        public void ModificaPromocion(int ActivarEnSams, int ActivarEnEdenred)
        {
            try
            {
                ActivarCadenaYSucursales();

                int progs = ActivarEnSams + ActivarEnEdenred;
                string Programa = ActivarEnSams == 1 ? "SamsBenefits" : ActivarEnEdenred == 1 ? "Edenred" : "";

                try
                {
                    for (int counter = 0; counter < progs; counter++)
                    {
                        LNPromociones.CreaPromocionEnAutorizador(Convert.ToInt32(hdnIdCadena.Value),
                            Convert.ToInt32(hdnIdPromo.Value), Programa, this.Usuario);

                        if (progs > counter + 1)
                        {
                            Programa = ActivarEnEdenred == 1 ? "Edenred" : "";
                        }
                    }

                    MessageBoxButtonsConfig mbbc = new MessageBoxButtonsConfig();
                    mbbc.Ok = new MessageBoxButtonConfig
                    {
                        Handler = "AltaPromo.CierraVentanaModuloPromo()",
                        Text = "Aceptar"
                    };

                    X.Msg.Alert("", "Promoción modificada <br /> <br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ", mbbc).Show();
                }

                catch (CAppException caEx)
                {
                    X.Msg.Alert("Módulo de Promociones", caEx.Mensaje()).Show();
                }

                catch (Exception ex)
                {
                    Loguear.Error(ex, this.Usuario.ClaveColectiva);
                    X.Msg.Alert("Módulo de Promociones", "Ocurrió un error al modificar la promoción").Show();
                }

                X.Mask.Hide();
            }

            catch (Exception err)
            {
                X.Mask.Hide();
                Loguear.Error(err, this.Usuario.ClaveUsuario);

                X.Msg.Alert("Módulo de Promociones", "Ocurrió un error en la modificación de la promoción").Show();
            }
        }

        /// <summary>
        /// Controla el llamado a los objetos de datos que activan y dan de alta la cadena,
        /// sucursales y demás colectivas necesarias en el Autorizador
        /// </summary>
        protected void ActivarCadenaYSucursales()
        {
            DataTable _dtSucursales = HttpContext.Current.Session["DtSucursalesPromo"] as DataTable;
            DataTable dtToDataBase = _dtSucursales.Clone();

            RowSelectionModel lasSucursales = this.GridSucursales.SelectionModel.Primary as RowSelectionModel;

            if (lasSucursales.SelectedRows.Count == 0)
            {
                X.Msg.Alert("Módulo de Promociones", "Por favor, selecciona al menos una sucursal.").Show();
                return;
            }

            foreach (SelectedRow sucursal in lasSucursales.SelectedRows)
            {
                foreach (DataRow row in _dtSucursales.Rows)
                {
                    if (sucursal.RecordID == row["ID_Sucursal"].ToString())
                    {
                        dtToDataBase.ImportRow(row);
                    }
                }
            }

            dtToDataBase.Columns.Remove("ActivaEnAutorizador");
            dtToDataBase.AcceptChanges();


            try
            {
                LNPromociones.InsertaSucursalesTemp(dtToDataBase, this.Usuario);
            }
            catch (CAppException caEx)
            {
                X.Mask.Hide();
                X.Msg.Alert("Módulo de Promociones", caEx.Mensaje()).Show();
            }
            catch (Exception err)
            {
                X.Mask.Hide();

                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Módulo de Promociones", "Ocurrió un error al validar las sucursales").Show();
            }

            try
            {
                LNPromociones.ModificaColectivasCadena(Convert.ToInt32(hdnIdCadena.Value), this.Usuario);
            }
            catch (CAppException caEx)
            {
                X.Mask.Hide();
                X.Msg.Alert("Módulo de Promociones", caEx.Mensaje()).Show();
            }
            catch (Exception err)
            {
                X.Mask.Hide();

                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Módulo de Promociones", "Ocurrió un error en la creación de la Cadena.").Show();
            }
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "AltaPromo")]
        public void StopMask()
        {
            X.Mask.Hide();
        }

        /// <summary>
        /// Controla el cierre de la ventanas del módulo de promociones cuando se modificó
        /// exitosamente alguna
        /// </summary>
        [DirectMethod(Namespace = "AltaPromo")]
        public void CierraVentanaModuloPromo()
        {
            WdwModuloPromos.Hide();

            LlenaGridPromociones(Convert.ToInt32(hdnIdCadena.Value));
        }

        /// <summary>
        /// Exporta el grid de resultados al backend, para generar y exportarlo a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            string gridResultJson = e.ExtraParams["GridResult"];
            string reportName = "UsuariosCadena";

            XmlNode gridResultXml = JSON.DeserializeXmlNode("{records:{record:" + gridResultJson + "}}");
            XmlTextReader xtr = new XmlTextReader(gridResultXml.OuterXml, XmlNodeType.Element, null);

            DataSet ds = new DataSet();
            ds.ReadXml(xtr);

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(reportName);

            //Se inserta la tabla completa a la hoja de Excel
            ws.Cell(1, 1).InsertTable(ds.Tables[0].AsEnumerable());

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

        /// <summary>
        /// Establece el formato deseado a las columnas de la hoja de trabajo por exportar
        /// </summary>
        /// <param name="ws">Hoja de trabajo</param>
        /// <param name="rowsNum">Total de filas de la hoja de trabajo</param>
        /// <returns></returns>
        protected IXLWorksheet FormatWsColumns(IXLWorksheet ws, int rowsNum)
        {
            ws.Column(1).Hide();

            return ws;
        }

        /// <summary>
        /// Controla el evento clic al botón de carga de archivo, controlando las validaciones
        /// del archivo y la carga en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnCargarArchivo_Click(object sender, DirectEventArgs e)
        {
            try
            {
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

                if (NumColumnas > 4)
                {
                    for (int iCounter = 4; iCounter < NumColumnas; iCounter++)
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
                    dtClonedShow.Columns[i].ColumnName = dtCloned.Columns[i].ColumnName;//.ToUpper();
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
                    for (int column = 0; column < 4; column++)
                    {
                        row[column] = row[column].ToString().Trim();
                    }

                    dtCloned.ImportRow(row);
                    dtClonedShow.ImportRow(row);
                }

                //Se verifica que en el archivo no existan correos duplicados
                if (ExistenCorreosDuplicados(dtCloned))
                {
                    return;
                }

                //Se verifica contenido y formato de cada columna
                if (VerificaFormatosArchivo(dtCloned, NumColumnas))
                {
                    return;
                }

                //El archivo pasó todas las validaciones
                dtCloned.AcceptChanges();
                dtClonedShow.AcceptChanges();

                LNPromociones.InsertaOperadoresTMP(dtCloned, this.Usuario);

                StoreAltaUsuarios.DataSource = dtClonedShow;
                StoreAltaUsuarios.DataBind();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Alta de Usuarios", ex.Message).Show();
            }
        }

        /// <summary>
        /// Verifica si existen direcciones de correo electrónico duplicadas
        /// </summary>
        /// <param name="_dtClonado">DataTable con la información del archivo Excel</param>
        /// <returns>TRUE si se encuentran duplicados</returns>
        protected bool ExistenCorreosDuplicados(DataTable _dtClonado)
        {
            Boolean resultado = false;

            var correosDuplicados = _dtClonado.AsEnumerable()
               .Select(dr => dr.Field<string>("EMAIL"))
               .GroupBy(x => x)
               .Where(g => g.Count() > 1)
               .Select(g => g.Key)
               .ToList();

            if (correosDuplicados.Count > 0)
            {
                string listaCorreos = null;
                string msj = null;

                foreach (String correo in correosDuplicados)
                {
                    listaCorreos += correo + "<br />";
                }

                if (correosDuplicados.Count > 1)
                {
                    msj = "Las siguientes cuentas de correo electrónico están duplicadas: <br /><b>" + listaCorreos +
                        "</b> <br /> Favor de verificar.";
                }
                else
                {
                    msj = "La siguiente cuenta de correo electrónico está duplicada: <br /><b>" + listaCorreos +
                        "</b> <br /> Favor de verificar.";
                }

                X.Msg.Alert("Error de contenido en archivo excel", msj).Show();
                resultado = true;
            }

            return resultado;
        }

        /// <summary>
        /// Verificando tipo de dato
        /// se verificara que el tipo de dato corresponda con lo que se pide en caso contrario regresara el error
        /// y la linea y columna a la que pertence
        /// </summary>
        /// <param name="_dtFile"></param>
        /// <param name="totalColumnas"></param>
        /// <returns></returns>
        protected bool VerificaFormatosArchivo(DataTable _dtFile, int totalColumnas)
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
                    switch (i)
                    {
                        case 2:
                            Match matchExpression;
                            Regex matchEmail = new Regex(regexEmail);

                            matchExpression = matchEmail.Match(row[i].ToString());

                            if (!matchExpression.Success)
                            {
                                X.Msg.Alert("Error de contenido en archivo excel", "El correo electrónico de la fila no.  <b>" + (fila + 2)
                                            + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                            + ")</b> no es una dirección válida").Show();
                                error = true;
                                i = totalColumnas;
                            }
                            break;

                        case 3:
                            if (row[i].ToString().ToUpper() == "SI" || row[i].ToString().ToUpper() == "NO"
                            || row[i].ToString().ToUpper().Equals("SÍ"))
                            {
                                if (row[i].ToString().ToUpper() == "SI" || row[i].ToString().ToUpper().Equals("SÍ"))
                                {
                                    row[i] = "1";
                                }
                                else
                                {
                                    row[i] = "0";
                                }
                            }
                            else
                            {
                                try
                                {
                                    int celda = int.Parse(row[i].ToString());
                                    if (celda != 0 && celda != 1)
                                    {
                                        X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                            + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                            + ")</b> debe ser numérica (1 ó 0) o en su defecto, la palabra SI o NO").Show();
                                        error = true;
                                        i = totalColumnas;
                                    }
                                }

                                catch (Exception)
                                {
                                    X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                            + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                            + ")</b> debe ser numérica (1 ó 0) o en su defecto, la palabra SI o NO").Show();
                                    error = true;
                                    i = totalColumnas;
                                }
                            }
                            break;

                        default:
                            if (string.IsNullOrEmpty(row[i].ToString()))
                            {
                                for (int f = fila; f < _dtFile.Rows.Count; f++)
                                {//este buscara que no haya blancos en las subsecuentes si hay hasta terminar el excel entonces es el final del documento y no debe marcar error

                                    if (string.IsNullOrEmpty(_dtFile.Rows[f][0].ToString()) && string.IsNullOrEmpty(_dtFile.Rows[f][1].ToString()))
                                    { }
                                    else
                                    {
                                        errorBlancos = 2;
                                        break;
                                    }
                                }

                                if (errorBlancos == 2)
                                {
                                    X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                        + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                        + ")</b> no puede estar vacía.").Show();
                                    error = true;
                                    i = totalColumnas;
                                }
                                else
                                {
                                    errorBlancos = 1;
                                    i = totalColumnas;
                                }
                            }
                            break;
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
        /// Controla el evento clic al botón de alta de usuarios, controlando la llamada al
        /// procedimiento en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnAltaUsuarios_Click(object sender, DirectEventArgs e)
        {
            try
            {
                LNPromociones.AltaUsuariosEnAutorizador(this.Usuario);

                MessageBoxButtonsConfig mbbc = new MessageBoxButtonsConfig();
                mbbc.Ok = new MessageBoxButtonConfig
                {
                    Handler = "AltaPromo.CierraVentanasAltaUsuarios()",
                    Text = "Aceptar"
                };

                X.Msg.Alert("Alta de Usuarios", "Usuarios creados <br />  <b> E X I T O S A M E N T E </b> <br />", mbbc).Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Alta de Usuarios", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Alta de Usuarios", "Ocurrió un Error al dar de Alta a los Usuarios").Show();
            }
        }

        /// <summary>
        /// Controla el evento clic al botón de crear usuario, controlando la llamada al
        /// procedimiento en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnCrearUsuario_Click(object sender, DirectEventArgs e)
        {
            try
            {
                Match matchExpression;
                Regex matchEmail = new Regex(regexEmail);

                matchExpression = matchEmail.Match(txtCorreo.Text);

                if (!matchExpression.Success)
                {
                    X.Msg.Alert("Alta de Usuario", "El correo electrónico proporcionado no es una dirección válida").Show();
                    return;
                }

                LNPromociones.AltaUsuarioEnAutorizador(hdnClaveCadena.Text, cBoxSucursal.SelectedItem.Value,
                    txtCorreo.Text, chkGerente.Checked ? 1 : 0, this.Usuario);

                MessageBoxButtonsConfig mbbc = new MessageBoxButtonsConfig();
                mbbc.Ok = new MessageBoxButtonConfig
                {
                    Handler = "AltaPromo.CierraVentanasAltaUsuarios()",
                    Text = "Aceptar"
                };

                X.Msg.Alert("Alta de Usuario", "Usuario creado <br />  <b> E X I T O S A M E N T E </b> <br />", mbbc).Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Alta de Usuario", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Alta de Usuario", "Ocurrió un Error al dar de Alta al Usuario").Show();
            }
        }

        /// <summary>
        /// Controla el cierre de las ventanas de alta de usuarios cuando ésta fue exitosa
        /// </summary>
        [DirectMethod(Namespace = "AltaPromo")]
        public void CierraVentanasAltaUsuarios()
        {
            WdwAltaUsuarios.Hide();
            WdwAltaUsuario.Hide();

            LlenaGridUsuarios(hdnClaveCadena.Text);
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de usuarios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            try
            {
                char[] charsToTrim = { '*', '"', '/' };
                String comando = (String)e.ExtraParams["Comando"];
                Int64 IdOperador = Convert.ToInt64(e.ExtraParams["ID_Colectiva"]);
                String usuario = (String)e.ExtraParams["Usuario"];
                String tipoAcceso = (String)e.ExtraParams["TipoAcceso"];

                switch (comando)
                {
                    case "Edita":
                        txtEmail.Value = usuario.Trim(charsToTrim);
                        rdGerente.Checked = tipoAcceso.ToUpper().Contains("GERENTE") ? true : false;
                        rdOperador.Checked = tipoAcceso.ToUpper().Contains("OPERADOR") ? true : false;

                        WdwEditaUsuario.Show();
                        break;

                    case "Elimina":
                        LNPromociones.EliminaUsuarioEnAutorizador(IdOperador, this.Usuario);

                        X.Msg.Notify("", "Usuario eliminado <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaGridUsuarios(hdnClaveCadena.Text);
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento clic al botón de aplicar cambio de la ventana de edición de usuario,
        /// controlando la llamada al procedimiento en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnAplicarCambio_Click(object sender, DirectEventArgs e)
        {
            try
            {
                LNPromociones.ModificaRolUsuario(txtEmail.Text, rdGerente.Checked ? 1 : 0, this.Usuario);

                X.Msg.Notify("", "Tipo de acceso modificado <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                WdwEditaUsuario.Hide();

                LlenaGridUsuarios(hdnClaveCadena.Text);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Edición de Usuario", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Edición de Usuario", "Ocurrió un Error al modificar al Usuario").Show();
            }
        }
    }
}