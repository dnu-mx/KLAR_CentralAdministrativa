using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.Entidades;
using DALPuntoVentaWeb.LogicaNegocio;
using DALPuntoVentaWeb.Utilidades;
using Excel;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace TpvWeb
{
    public partial class ImportarDatosExcel : DALCentralAplicaciones.PaginaBaseCAPP
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
            }

            catch (Exception err)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }


        /// <summary>
        /// Controla el evento Click al botón de Importar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directo del evento que se ejecutó</param>
        public void btnImportar_Click(object sender, DirectEventArgs e)
        {
            try
            {
                HttpPostedFile file = this.FileUploadField1.PostedFile;

                string[] directorios = file.FileName.Split('\\');
                string fileName = directorios[directorios.Count() - 1];

                //Se valida que se haya seleccionado un archivo
                if (String.IsNullOrEmpty(fileName))
                {
                    X.Msg.Alert("Importar Archivo", "Selecciona un archivo para importarlo.").Show();
                    return;
                }

                //Se valida que se haya seleccionado un archivo Excel
                if (!fileName.Contains(".xlsx"))
                {
                    X.Msg.Alert("Importar Archivo", "El archivo seleccionado no es del formato Excel soportado (*.xlsx). Verifica tu archivo.").Show();
                    return;
                }

                string tempPath = "C:\\TmpXlsFiles\\";

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

                ActualizaDatosEnBD(dsArchivo);

                X.Msg.Notify("Importar Archivo", "Archivo importado <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException caEx)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Importar Archivo", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Importar Archivo", "Ocurrió un error al importar el archivo.").Show();
            }
        }

       /// <summary>
        /// Procesa cada registro del archivo para enviarlo a actualización en base de datos
       /// </summary>
       /// <param name="dsRegistros">DataSet con el archivo recibido</param>
        protected void ActualizaDatosEnBD(DataSet dsRegistros)
        {
            try
            {
                DatosSrPago losDatos = new DatosSrPago();

                //Obtenemos de la configuración de la aplicación los nombres de las columnas;
                String claveAlmacen = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaClaveAlmacen").Valor.Trim();

                String claveTienda = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaClaveTienda").Valor.Trim();

                String calle = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaCalle").Valor.Trim();

                String localidad = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaLocalidad").Valor.Trim();

                String municipio = Configuracion.Get(
                   Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaMunicipio").Valor.Trim();

                String estado = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaEstado").Valor.Trim();           
           
                String telefono = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaTelefono").Valor.Trim();

                String email = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaEmail").Valor.Trim();

                String password = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaPasswordSrPago").Valor.Trim();

                String nombreOperador = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaNombreOperador").Valor.Trim();

                String apPatOperador = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaApPaternoOperador").Valor.Trim();

                String apMatOperador = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaApMaternoOperador").Valor.Trim();

                String cp = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaCP").Valor.Trim();

                String numTarjeta = Configuracion.Get(
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ColumnaNumTarjetaDebitoSrPago").Valor.Trim();


                //Se validan y almacenan los registros
                for (int registro = 1; registro < dsRegistros.Tables[0].Rows.Count; registro++)
                {
                    losDatos = new DatosSrPago();
                    
                    losDatos.ClaveAlmacen = dsRegistros.Tables[0].Rows[registro][claveAlmacen].ToString().Trim();
                    losDatos.ClaveTienda = dsRegistros.Tables[0].Rows[registro][claveTienda].ToString().Trim().PadLeft(4, '0');
                    losDatos.Calle = dsRegistros.Tables[0].Rows[registro][calle].ToString().Trim();
                    losDatos.Localidad = dsRegistros.Tables[0].Rows[registro][localidad].ToString().Trim();
                    losDatos.Municipio = dsRegistros.Tables[0].Rows[registro][municipio].ToString().Trim();
                    losDatos.Estado = dsRegistros.Tables[0].Rows[registro][estado].ToString().Trim();
                    losDatos.Telefono = dsRegistros.Tables[0].Rows[registro][telefono].ToString().Trim();
                    losDatos.Email = dsRegistros.Tables[0].Rows[registro][email].ToString().Trim();
                    losDatos.Password = dsRegistros.Tables[0].Rows[registro][password].ToString().Trim();
                    losDatos.NombreOperador = dsRegistros.Tables[0].Rows[registro][nombreOperador].ToString().Trim();
                    losDatos.ApellidoPaternoOperador = dsRegistros.Tables[0].Rows[registro][apPatOperador].ToString().Trim();
                    losDatos.ApellidoMaternoOperador = dsRegistros.Tables[0].Rows[registro][apMatOperador].ToString().Trim();
                    losDatos.CodigoPostal = dsRegistros.Tables[0].Rows[registro][cp].ToString().Trim();
                    losDatos.NumeroTarjeta = dsRegistros.Tables[0].Rows[registro][numTarjeta].ToString().Trim();

                    //Se incluye esta validación pues el número de filas no es preciso en archivos Excel
                    if (String.IsNullOrEmpty(losDatos.ClaveAlmacen))
                    {
                        break;
                    }

                    LNSrPago.ActualizaDatosDeArchivoEnBD(losDatos, this.Usuario);
                }
            }

            catch (CAppException caEx)
            {
                throw new CAppException(8001, "ActualizaDatosEnBD() " + caEx);
            }

            catch (Exception ex)
            {
                throw new Exception("ActualizaDatosEnBD() " + ex);
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            FileUploadField1.Reset();
        }
    }
}