using DALBovedaTarjetas.LogicaNegocio;
using DALCentralAplicaciones;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace BovedaTarjetas
{
    public partial class SolicTarjetasNominativas : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Bóveda Solicitar Tarjetas Nominativas
        private LogHeader LH_BovedaSolicTarjNom = new LogHeader();

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_BovedaSolicTarjNom.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_BovedaSolicTarjNom.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_BovedaSolicTarjNom.User = this.Usuario.ClaveUsuario;
            LH_BovedaSolicTarjNom.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_BovedaSolicTarjNom);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA SolicTarjetasNominativas Page_Load()");

                if (!IsPostBack)
                {
                }

                log.Info("TERMINA SolicTarjetasNominativas Page_Load()");
            }
            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
                errRedirect = "../ErrorInicializarPagina.aspx";
            }

            catch (Exception err)
            {
                log.ErrorException(err);
                errRedirect = "../ErrorInicializarPagina.aspx";
            }

            finally
            {
                if (!string.IsNullOrEmpty(errRedirect))
                {
                    Response.Redirect(errRedirect, false);

                    Response.Flush();
                    Response.Close();
                }
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        public void btnCargarArchivo_Click(object sender, DirectEventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_BovedaSolicTarjNom);
            logPCI.Info("btnCargarArchivo_Click()");

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
                HttpPostedFile file = this.FileUploadField1.PostedFile;

                string[] directorios = file.FileName.Split('\\');
                string fileName = directorios[directorios.Count() - 1];

                //Se valida que se haya seleccionado un archivo
                if (String.IsNullOrEmpty(fileName))
                {
                    throw new CAppException(8012, "Selecciona un archivo para cargarlo.");
                }

                //Se valida que se haya seleccionado un archivo de texto
                if (!fileName.Contains(".txt"))
                {
                    throw new CAppException(8012, "El archivo seleccionado no es del formato de texto soportado (*.txt). Verifica tu archivo.");
                }

                //Se valida que se haya seleccionado un archivo de texto
                if (fileName.Substring(0,10) != "PERSOCARDS_")
                {
                    throw new CAppException(8012, "El prefijo del nombre del archivo seleccionado, no corresponde al formato requerido (PERSOCARDS_). Verifica tu archivo.");
                }

                string tempPath = "C:\\TmpTxTFiles\\";

                //Si no existe el directorio, lo crea
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                tempPath += fileName;

                //Se almacena archivo en directorio temporal del que es dueño el aplicativo,
                //para no tener problemas de permisos de apertura
                file.SaveAs(tempPath);

                FileInfo InfoArchivo = new FileInfo(tempPath);

                //if (!LNBoveda.validarContenido(InfoArchivo, LH_BovedaSolicTarjNom))
                //{
                //    LNBoveda.validarContenidoCredito(InfoArchivo, LH_BovedaSolicTarjNom);
                //}
               


                    X.Msg.Notify("Cargar Archivo", "Archivo cargado correctamente.<br/>Selecciona Aplicar Cambios").Show();


                //GridDatosArchivo.GetStore().DataSource = dtClonedShow;
                //GridDatosArchivo.GetStore().DataBind();
            }
            catch (CAppException caEx)
            {
                logPCI.Error(caEx.Mensaje());
                X.Msg.Alert("Cargar Archivo", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Cargar Archivo", "Ocurrió un error durante la carga del archivo.").Show();
            }
        }

    }
}