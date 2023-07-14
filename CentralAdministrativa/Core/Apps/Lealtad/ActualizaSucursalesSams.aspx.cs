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
    public partial class ActualizaSucursalesSams : DALCentralAplicaciones.PaginaBaseCAPP
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
        /// Controla el evento Click al botón Cargar Archivo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directo del evento que se ejecutó</param>
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

                ValidaMuestraYSubeArchivo(dsArchivo);
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                //X.Msg.Alert("Actualizar Sucursales", "Ocurrió un error al cargar el archivo.").Show();
                X.Msg.Alert("Actualizar Sucursales", ex.Message).Show();
            }
        }

        /// <summary>
        /// Realiza la validación de los datos en la tabla, los almacena en la tabla
        /// temporal de base de datos y los muestra el el grid correspondiente.
        /// </summary>
        /// <param name="ds">DataSet con los datos del archivo Excel</param>
        protected void ValidaMuestraYSubeArchivo(DataSet ds)
        {
            Int32 cp = 0, rowIndex = 0, finalRow;
            Decimal latitud, longitud = 0;

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
                    X.Msg.Alert("Actualizar Sucursales",
                        "La Clave de la Cadena no puede estar vacía. <b>(Fila no. " +
                        (rowIndex + 2).ToString() + ")</b>").Show();
                    return;
                }

                if (String.IsNullOrEmpty(row.ItemArray[1].ToString()))
                {
                    X.Msg.Alert("Actualizar Sucursales",
                        "La Clave de la Sucursal no puede estar vacía. <b>(Fila no. " +
                        (rowIndex + 2).ToString() + ")</b>").Show();
                    return;
                }

                if (String.IsNullOrEmpty(row.ItemArray[2].ToString()))
                {
                    X.Msg.Alert("Actualizar Sucursales",
                        "El Nombre de la Sucursal no puede estar vacío. <b>(Fila no. " +
                        (rowIndex + 2).ToString() + ")</b>").Show();
                    return;
                }

                if (String.IsNullOrEmpty(row.ItemArray[3].ToString()))
                {
                    X.Msg.Alert("Actualizar Sucursales",
                        "La Dirección no puede estar vacía. <b>(Fila no. " +
                        (rowIndex + 2).ToString() + ")</b>").Show();
                    return;
                }

                if (!String.IsNullOrEmpty(row.ItemArray[6].ToString()))
                {
                    if (!Int32.TryParse(row.ItemArray[6].ToString(), out cp))
                    {
                        X.Msg.Alert("Actualizar Sucursales",
                            "El Código Postal debe ser numérico. <b>(Fila no. " +
                            (rowIndex + 2).ToString() + ")</b>").Show();
                        return;
                    }
                }

                if (String.IsNullOrEmpty(row.ItemArray[7].ToString()))
                {
                    X.Msg.Alert("Actualizar Sucursales",
                        "El Estado no puede estar vacío. <b>(Fila no. " +
                        (rowIndex + 2).ToString() + ")</b>").Show();
                    return;
                }

                if (!String.IsNullOrEmpty(row.ItemArray[9].ToString()))
                {
                    if (!Decimal.TryParse(row.ItemArray[9].ToString(), out latitud))
                    {
                        X.Msg.Alert("Actualizar Sucursales", "Latitud inválida.<b>(Fila no. " +
                            (rowIndex + 2).ToString() + ")</b>").Show();
                        return;
                    }
                    else if (latitud < -90 || latitud > 90)
                    {
                        X.Msg.Alert("Actualizar Sucursales", "Latitud inválida.<b>(Fila no. " +
                            (rowIndex + 2).ToString() + ")</b>").Show();
                        return;
                    }
                }

                if (!String.IsNullOrEmpty(row.ItemArray[10].ToString()))
                {
                    if (!Decimal.TryParse(row.ItemArray[10].ToString(), out longitud))
                    {
                        X.Msg.Alert("Actualizar Sucursales", "Longitud inválida <b>(Fila no. " +
                            (rowIndex + 2).ToString() + ")</b>").Show();
                        return;
                    }
                    else if (longitud < -15069 || longitud > 15069)
                    {
                        X.Msg.Alert("Actualizar Sucursales", "Longitud inválida <b>(Fila no. " +
                            (rowIndex + 2).ToString() + ")</b>").Show();
                        return;
                    }
                }

                if (String.IsNullOrEmpty(row.ItemArray[11].ToString()))
                {
                    X.Msg.Alert("Actualizar Sucursales",
                        "Activa no puede estar vacía. <b>(Fila no. " +
                        (rowIndex + 2).ToString() + ")</b>").Show();
                    return;
                }
                else
                {
                    dt.Rows[rowIndex][11] = row.ItemArray[11].ToString().Trim().ToUpper() == "SI" ? 1 : 0;
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
            LNEcommerceSams.InsertaSucursalesTMP(dt, this.Usuario);

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
            dt_.Columns[0].ColumnName = "ClaveCadena";
            dt_.Columns[1].ColumnName = "ClaveSucursal";
            dt_.Columns[2].ColumnName = "NombreSucursal";
            dt_.Columns[3].ColumnName = "Direccion";
            dt_.Columns[4].ColumnName = "Colonia";
            dt_.Columns[5].ColumnName = "Ciudad";
            dt_.Columns[6].ColumnName = "CP";
            dt_.Columns[7].ColumnName = "Estado";
            dt_.Columns[8].ColumnName = "Telefono";
            dt_.Columns[9].ColumnName = "Latitud";
            dt_.Columns[10].ColumnName = "Longitud";
            dt_.Columns[11].ColumnName = "Activa";
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
                string respuestaBD = LNEcommerceSams.AplicaCambiosASucursales(this.Usuario);

                if (respuestaBD.Contains("Error"))
                {
                    X.Msg.Alert("Actualizar Sucursales", respuestaBD + "<br /> ").Show();
                }
                else
                {
                    X.Msg.Notify("Actualizar Sucursales", respuestaBD + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                } 
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Sucursales", "Ocurrió un error al aplicar los cambios en base de datos.").Show();
            }
        }
    }
}