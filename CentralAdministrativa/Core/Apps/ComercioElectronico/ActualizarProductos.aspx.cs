using DALComercioElectronico.LogicaNegocio;
using DALComercioElectronico.Utilidades;
using Excel;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace ComercioElectronico
{
    public partial class ActualizarProductos : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }


        /// <summary>
        /// Controla el evento Click al botón de Cargar Archivo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directo del evento que se ejecutó</param>
        public void btnCargarArchivo_Click(object sender, DirectEventArgs e)
        {
            try
            {
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
                DataTable dtCloned = dsArchivo.Tables[0].Clone();

                int val = 0;

                foreach (DataRow row in dsArchivo.Tables[0].Rows)
                {
                    for (int i = 0; i < row.ItemArray.Length; i++)
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
                    dtCloned.ImportRow(row);
                }

                LNMoshi.InsertaArchivoTMP(dtCloned, this.Usuario);

                GridDatosArchivo.GetStore().DataSource = dsArchivo;
                GridDatosArchivo.GetStore().DataBind();
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Productos", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                //X.Msg.Alert("Actualizar Productos", "Ocurrió un error al cargar el archivo.").Show();
                X.Msg.Alert("Actualizar Productos", ex.Message).Show();
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
                LNMoshi.InsertaValoresAtributos(this.Usuario);

                var response = LNMoshi.AplicaCambios(this.Usuario);

                X.Msg.Notify("Actualizar Productos", response + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Productos", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Productos", "Ocurrió un error al aplicar los cambios en base de datos.").Show();
            }
        }
    }
}