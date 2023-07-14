using DALAutorizador.Utilidades;
using DALLealtad.BaseDatos;
using DALLealtad.LogicaNegocio;
using Excel;
using Ext.Net;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace Lealtad
{
    public partial class ActualizaBeneficiosSams : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
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

                if (NumColumnas > 17)
                {
                    for (int iCounter = 17; iCounter < NumColumnas; iCounter++)
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
                    for (int column = 0; column < 17; column++)
                    {
                        row[column] = row[column].ToString().Trim();
                    }

                    dtCloned.ImportRow(row);
                    dtClonedShow.ImportRow(row);
                }

                dtCloned.Columns[15].ColumnName = "FechaInicio";
                dtCloned.Columns[16].ColumnName = "FechaFin";

                dtClonedShow.Columns[15].ColumnName = "FECHAINICIO(DD/MM/AAAA)";
                dtClonedShow.Columns[16].ColumnName = "FECHAFIN(DD/MM/AAAA)";

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
                if (VerificaFormatosArchivo(dtCloned, NumColumnas))
                {
                    return;
                }

                //CASO ESPECIAL: Se da formato a los campos de fecha para mostrarlos en el grid
                EstableceFormatoFechas(dtClonedShow);


                //El archivo pasó todas las validaciones
                dtCloned.AcceptChanges();

                LNEcommerceSams.InsertaArchivoTMP(dtCloned, this.Usuario);
                X.Msg.Notify("Cargar Archivo", "Archivo cargado correctamente.<br/>Selecciona Aplicar Cambios").Show();

                GridDatosArchivo.GetStore().DataSource = dtClonedShow;
                GridDatosArchivo.GetStore().DataBind();
            }

            catch (Exception ex)
            {
                 Loguear.Error(ex, this.Usuario.ClaveUsuario);
                //X.Msg.Alert("Actualizar Productos", "Ocurrió un error al cargar el archivo.").Show();
                X.Msg.Alert("Actualizar Beneficios", ex.Message).Show();
            }
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
               .Select(dr => dr.Field<string>("CLAVECADENA"))
               .GroupBy(x => x)
               .Where(g => g.Count() > 1)
               .Select(g => g.Key)
               .ToList();

            if (clavesDuplicadas.Count > 0)
            {
                int i = 0;
                int rowIndex;
                DataTable dtDuplicadas = new DataTable();

                dtDuplicadas.Columns.Add("ClaveCadena");
                dtDuplicadas.Columns.Add("Cadena");
                dtDuplicadas.Columns.Add("Giro");

                foreach (String clave in clavesDuplicadas)
                {
                    foreach (DataRow row in _dtClonado.Rows)
                    {
                        if (row.ItemArray[0].ToString() == clave)
                        {
                            dtDuplicadas.Rows.Add();

                            dtDuplicadas.Rows[i]["ClaveCadena"] = clave;
                            dtDuplicadas.Rows[i]["Cadena"] = row.ItemArray[1].ToString();
                            dtDuplicadas.Rows[i]["Giro"] = row.ItemArray[2].ToString();

                            i++;
                            break;
                        }
                    }
                }

                foreach (DataRow rowdup in dtDuplicadas.Rows)
                {
                    rowIndex = 2;

                    foreach (DataRow row in _dtClonado.Rows)
                    {
                        if (rowdup["ClaveCadena"].ToString() == row.ItemArray[0].ToString())
                        {
                            if (rowdup["Cadena"].ToString() != row.ItemArray[1].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>Nombre de la Cadena</b> no es el mismo en todas las promociones con la Clave de Cadena <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }
                            else if (rowdup["Giro"].ToString() != row.ItemArray[2].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>Giro</b> no es el mismo en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }
                        }

                        rowIndex++;
                    }

                    if (resultado) break;
                }
            }

            return resultado;
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
            int ClaveCadena = 0, Cadena = 0, ClavePromocion = 0, TituloPromocion = 0;
            int TipoDescuento = 0, DescripcionBeneficio = 0, RedesSociales = 0;
            int rowIndex = 2;

            try
            {
                DataSet dsLongitudes = DAOEcommerceSams.ObtieneLongitudCampos(this.Usuario);

                foreach (DataRow dr in dsLongitudes.Tables[0].Rows)
                {
                    switch (dr["NombreCampo"])
                    {
                        case "ClaveCadena": ClaveCadena = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "Cadena": Cadena = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "ClavePromocion": ClavePromocion = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "TituloPromocion": TituloPromocion = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "TipoDescuento": TipoDescuento = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "DescripcionBeneficio": DescripcionBeneficio = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "RedesSociales": RedesSociales = Convert.ToInt32(dr["LongitudCampo"]); break;
                    }
                }

                foreach (DataRow row in _dt.Rows)
                {
                    if (row["CLAVECADENA"].ToString().Length > ClaveCadena)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la <b>Clave de Cadena</b> excede el número máximo permitido de " + ClaveCadena.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["CADENA"].ToString().Length > Cadena)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la <b>Cadena</b> excede el número máximo permitido de " + Cadena.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["CLAVEPROMO"].ToString().Length > ClavePromocion)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la <b>Clave de Promoción</b> excede el número máximo permitido de " + ClavePromocion.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["TITULOPROMOCION"].ToString().Length > TituloPromocion)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud del <b>Título de la Promoción</b> excede el número máximo permitido de " + TituloPromocion.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["TIPODESCUENTO"].ToString().Length > TipoDescuento)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud del <b>Tipo de Descuento</b> excede el número máximo permitido de " + TipoDescuento.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["DESCRIPCIONBENEFICIO"].ToString().Length > DescripcionBeneficio)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la <b>Descripción del Beneficio</b> excede el número máximo permitido de " + DescripcionBeneficio.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["FACEBOOK"].ToString().Length > RedesSociales)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>Facebook</b> excede el número máximo permitido de " + RedesSociales.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["WEB"].ToString().Length > RedesSociales)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>Web</b> excede el número máximo permitido de " + RedesSociales.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }

                    rowIndex++;
                }
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Beneficios", ex.Message).Show();
            }

            return validacion;
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
                    if (i <= 3 || i == 6)
                    {
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
                                break;
                            }
                            else
                            {
                                errorBlancos = 1;
                                break;
                            }
                        }
                    }

                    else if (i == 10 || i == 14)
                    {
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
                                    break;
                                }

                            }
                            catch (Exception)
                            {
                                X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                        + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                        + ")</b> debe ser numérica (1 ó 0) o en su defecto, la palabra SI o NO").Show();
                                error = true;
                                break;
                            }

                        }
                    }

                    else if (i == 11 || i == 12)
                    {
                        row[i] = string.IsNullOrEmpty(row[i].ToString()) ? "0" : row[i];

                        if (row[i].ToString().ToUpper() == "NO")
                        {
                            row[i] = "0";
                        }

                        else
                        {
                            try
                            {
                                int celda = int.Parse(row[i].ToString());
                            }
                            catch (Exception)
                            {
                                X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                    + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                    + ")</b> debe ser numérica o en su defecto, la palabra NO").Show();
                                error = true;
                                break;
                            }

                        }
                    }

                    else if (i == 13)
                    {
                        row[i] = string.IsNullOrEmpty(row[i].ToString()) ? "0" : row[i];

                        try
                        {
                            int celda = int.Parse(row[i].ToString());
                        }
                        catch (Exception)
                        {
                            X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                + ")</b> debe ser numérica").Show();
                            error = true;
                            break;
                        }
                    }

                    else if (i == 15 || i == 16)
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
                    if (dc.ColumnName.Contains("FECHAINICIO") || dc.ColumnName.Contains("FECHAFIN"))
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
        /// Controla el evento Click al botón Aplicar Cambios del Grid principal
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directo del evento que se ejecutó</param>
        public void btnAplicarCambios_Click(object sender, DirectEventArgs e)
        {
            try
            {
                var response = LNEcommerceSams.AplicaCambios(this.Usuario);
                if (response.Contains("Error"))
                {
                    X.Msg.Alert("Actualizar Beneficios", response + "<br /> ").Show();
                  //  Loguear.Error(ex, this.Usuario.ClaveUsuario);

                }
                else
                {
                    X.Msg.Alert("Actualizar Beneficios", response + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                }
              
            }
            
            catch (Exception ex)
            {
                 Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Beneficios", "Ocurrió un error al aplicar los cambios en base de datos.").Show();
            }
        }
    }
}