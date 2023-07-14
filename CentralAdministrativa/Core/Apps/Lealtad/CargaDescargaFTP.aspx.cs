using DALCentralAplicaciones.BaseDatos;
using DALLealtad.LogicaNegocio;
using DALLealtad.Utilidades;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace Lealtad
{
    public partial class CargaDescargaFTP : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Carga y Descarga FTP
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    StoreCarpetasFTP.DataSource =
                        DAOPropiedad.ObtieneCarpetasFTP(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    StoreCarpetasFTP.DataBind();
                }

                if (!X.IsAjaxRequest)
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
        /// Controla el evento Click al botón Obtener Archivos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directo del evento que se ejecutó</param>
        public void btnObtieneArchivos_Click(object sender, DirectEventArgs e)
        {
            try
            {
                string tempPath, root;
                root = "C:\\TmpFTPFiles\\";

                if (Request.Files.Count > 0)
                {
                    LimpiaYCreaDirectorio();

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFile file = Request.Files[i];

                        tempPath = root + file.FileName;

                        file.SaveAs(tempPath);
                    }

                    StoreOrigen.RemoveAll();
                    string[] files = Directory.GetFiles(root);

                    List<object> data = new List<object>(files.Length);

                    foreach (string fileName in files)
                    {
                        FileInfo _file = new FileInfo(fileName);

                        data.Add(new
                        {
                            NombreArchivo = _file.Name,
                            Tamanyo = _file.Length
                        });
                    }

                    StoreOrigen.DataSource = data;
                    StoreOrigen.DataBind();

                    RowSelectionModel losArchivos = this.GridPanelOrigen.SelectionModel.Primary as RowSelectionModel;
                    losArchivos.SelectAll();
                }
                else
                {
                    throw new Exception("Ocurrió un error al obtener los archivos.");
                }
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Añadir Archivo", ex.Message).Show();
            }
        }

        /// <summary>
        /// Valida la existencia del directorio temporal de carga de archivos;
        /// si no existe, lo crea. Si el directorio ya existe, borra su contenido
        /// </summary>
        protected void LimpiaYCreaDirectorio()
        {
            string root = "C:\\TmpFTPFiles\\";

            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            else
            {
                DirectoryInfo di = new DirectoryInfo(root);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }

        /// <summary>
        /// Establece la conexión al servidor/carpeta FTP seleccionado y obtiene la lista
        /// de directorios contenidos en él
        /// </summary>
        /// <param name="Servidor">Servidor y carpeta del repositorio FTP</param>
        /// <param name="Usuario">Usuario para conectarse al repositorio FTP</param>
        /// <param name="Password">Contraseña para conectarse al repositorio FTP</param>
        /// <param name="SSL">Bandera que indica si se requiere o no el uso de SLL en la conexión FTP</param>
        [DirectMethod(Namespace = "FTP")]
        public void EstableceConexionFTP(string serv, string usr, string pwd, string ssl)
        {
            try
            {
                List<object> lasCarpetas = LNConexionFTP.ObtieneListaDirectoriosFTP(serv, usr, pwd, ssl);

                X.Mask.Hide();

                StoreDestino.DataSource = lasCarpetas;
                StoreDestino.DataBind();

                //Oculta el check selection model
                GridPanelDestino.ColumnModel.SetHidden(0, true);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Conexión FTP", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla y solicita la carga de archivos en el repositorio FTP
        /// </summary>
        [DirectMethod(Namespace = "UploadFTP", Timeout = 120000)]
        public void CargarArchivos()
        {
            try
            {
                string root = "C:\\TmpFTPFiles\\";
                string ClavesCadenas = "";
                string Coma = "";

                List<string> files = new List<string>();
                RowSelectionModel losArchivos = this.GridPanelOrigen.SelectionModel.Primary as RowSelectionModel;

                foreach (SelectedRow archivo in losArchivos.SelectedRows)
                {
                    ClavesCadenas = ClavesCadenas + Coma + archivo.RecordID.Substring(0, 5);
                    files.Add(archivo.RecordID);
                    Coma = ",";
                }

                LNConexionFTP.SubeArchivosADirectorioFTP(this.hdnSrvr.Value.ToString(), this.hdnUsr.Value.ToString(),
                    this.hndPw.Value.ToString(), this.hdnSsl.Value.ToString(), files, root);

                DataSet dsDatos = DAOPropiedad.ObtienePrograma(this.hdnSrvr.Value.ToString(), this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                /*Si no tiene programa no debe (ni puede) actualizar campo de carpeta logos.*/
                if (dsDatos.Tables[0].Rows.Count != 0)
                {
                    LNEcommercePrana.ActualizaExisteLogoCadenas(ClavesCadenas, dsDatos.Tables[0].Rows[0]["ValorPrograma"].ToString().Trim(), this.Usuario);
                }

                X.Mask.Hide();

                X.Msg.Notify("", "Los archivos se subieron <br /> <br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                EstableceConexionFTP(this.hdnSrvr.Value.ToString(), this.hdnUsr.Value.ToString(),
                    this.hndPw.Value.ToString(), this.hdnSsl.Value.ToString());

            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Subir Archivos", ex.Message).Show();
            }
        }
    }
}