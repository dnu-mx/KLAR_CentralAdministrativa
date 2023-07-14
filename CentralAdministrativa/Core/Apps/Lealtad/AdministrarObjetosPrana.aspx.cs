using DALCentralAplicaciones;
using DALCentralAplicaciones.BaseDatos;
using DALLealtad.BaseDatos;
using DALLealtad.Entidades;
using DALLealtad.LogicaNegocio;
using DALLealtad.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;

namespace Lealtad
{
    public partial class AdministrarObjetosPrana : PaginaBaseCAPP
    {
        /// <summary>
        /// Carga de la página Administrar la configuración de objetos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    StoreProgramas.DataSource = DAOEcommercePrana.ListaProgramas(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), false);
                    StoreProgramas.DataBind();

                    StoreTipoObjeto.DataSource = DAOEcommercePrana.ListaTipoObjeto(this.Usuario, false);
                    StoreTipoObjeto.DataBind();

                    StoreEntidades.DataSource = DAOEcommercePrana.ListaEntidades(this.Usuario, false);
                    StoreEntidades.DataBind();

                    cBoxPrograma.SelectedIndex = 0;
                    cBoxTipoObjeto.SelectedIndex = 0;
                    cBoxTipoEntidad.SelectedIndex = 0;
                    cBoxTipoEntidad_Update.SelectedIndex = 0;
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenaGridResultados();
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del panel para Editar y Agregar un Objeto
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddObjeto_Click(object sender, EventArgs e)
        {
            try
            {
                string ArchivoFTP = CargaArchivoFTP(0);
                AgregarObjeto(ArchivoFTP);
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Objeto", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Actualización de Objeto", ex.Message).Show();
            }
        }

        private void LlenaGridResultados()
        {
            try
            {
                DataSet dsObjetos = DAOEcommercePrana.ObtieneListaObjetos(this.Usuario, Convert.ToInt32(this.cBoxPrograma.Value),
                    Convert.ToInt32(this.cBoxTipoObjeto.Value));

                if (dsObjetos.Tables[0].Rows.Count == 0)
                {
                    StoreObjetos.RemoveAll();
                    X.Msg.Alert("Objetos", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreObjetos.DataSource = dsObjetos;
                    StoreObjetos.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Objetos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Objetos", ex.Message).Show();
            }
        }

        private void AgregarObjeto(string ArchivoFTP)
        {
            ObjetoPrograma objetoPrograma = new ObjetoPrograma();
            string ClaveObjeto = "";

            /*Header*/
            objetoPrograma.ID_Programa = Convert.ToInt32(cBoxPrograma.Value);
            objetoPrograma.ID_TipoObjeto = Convert.ToInt32(cBoxTipoObjeto.Value);
            objetoPrograma.ID_Entidad = null;

            /*Detalle*/
            objetoPrograma.ID_TipoEntidad = Convert.ToInt32(cBoxTipoEntidad.Value);
            objetoPrograma.Orden = nmbOrden.Text;
            objetoPrograma.URL = txtURL.Text;

            DataSet dtTiposObjetos = DAOEcommercePrana.ListaTipoObjeto(this.Usuario, false);

            foreach (DataRow row in dtTiposObjetos.Tables[0].Rows)
            {
                if (row["ID_tipoObjeto"].ToString() == cBoxTipoObjeto.Value.ToString())
                {
                    ClaveObjeto = row["Clave"].ToString();
                }
            }
            objetoPrograma.PathImagen = "/static/images/" + ClaveObjeto + "/" + ArchivoFTP;

            DataTable dtNuevoObjeto = LNEcommercePrana.CreaNuevoObjeto(objetoPrograma, this.Usuario);

            string msj = dtNuevoObjeto.Rows[0]["Mensaje"].ToString();
            int idNuevoObjeto = Convert.ToInt32(dtNuevoObjeto.Rows[0]["IdNuevoObjeto"]);

            if (idNuevoObjeto == -1)
            {
                X.Msg.Alert("Nuevo Objeto", msj).Show();
            }
            else
            {
                Modal_AltaObjeto.Hide();
                LlenaGridResultados();

                X.Msg.Notify("", "Nuevo Objeto Agregado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }
        }

        protected void btnEditaObjeto_Click(object sender, EventArgs e)
        {
            try
            {
                ObjetoPrograma objetoPrograma = new ObjetoPrograma();
                string ArchivoFTP = "";
                string ClaveObjeto = "";

                if (FileUF_Archivos_Update.HasFile) 
                {
                    ArchivoFTP = CargaArchivoFTP(1);
                }

                /*Header*/
                objetoPrograma.ID_Programa = Convert.ToInt32(cBoxPrograma.Value);
                objetoPrograma.ID_TipoObjeto = Convert.ToInt32(cBoxTipoObjeto.Value);
                objetoPrograma.ID_ProgramaObjeto = Convert.ToInt32(hdnIdProgramaObjeto.Text);
                objetoPrograma.ID_Entidad = null;

                /*Detalle*/
                objetoPrograma.ID_TipoEntidad = Convert.ToInt32(cBoxTipoEntidad_Update.Value);
                objetoPrograma.Orden = nmbOrden_Update.Text;
                objetoPrograma.URL = txtURL_Update.Text;
                objetoPrograma.Activo = 1;

                if (ArchivoFTP == "") {
                    objetoPrograma.PathImagen = hdnlblFile_Update.Text;
                }
                else
                {
                    DataSet dtTiposObjetos = DAOEcommercePrana.ListaTipoObjeto(this.Usuario, false);

                    foreach (DataRow row in dtTiposObjetos.Tables[0].Rows)
                    {
                        if (row["ID_tipoObjeto"].ToString() == cBoxTipoObjeto.Value.ToString())
                        {
                            ClaveObjeto = row["Clave"].ToString();
                        }
                    }
                    objetoPrograma.PathImagen = "/static/images/" + ClaveObjeto + "/" + ArchivoFTP;
                }

                DataTable dtObjetoMod =  LNEcommercePrana.ModificaObjeto(objetoPrograma, this.Usuario);
                string msj = dtObjetoMod.Rows[0]["Mensaje"].ToString();

                if (msj != "")
                {
                    Modal_EditaObjeto.Hide();
                    X.Msg.Alert("Actualización de Objeto", msj).Show();
                }
                else
                {
                    LlenaGridResultados();
                    Modal_EditaObjeto.Hide();
                    X.Msg.Notify("", "Objeto Actualizado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Objeto", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Actualización de Objeto", ex.Message).Show();
            }
        }

        [DirectMethod]
        public void Editar_Event(int idProgramasObjetos, int idTipoEntidad, string URL, string Orden, string PathImagen, string TipoObjetos_Desc)
        {
            lblIDProgramaObjeto.Text = idProgramasObjetos.ToString();
            hdnIdProgramaObjeto.Text = idProgramasObjetos.ToString();

            nmbOrden_Update.Text = Orden;
            cBoxTipoEntidad_Update.SelectedItem.Value = idTipoEntidad.ToString();
            txtURL_Update.Text = URL;
            lblFile_Update.Text = PathImagen;
            hdnlblFile_Update.Text = PathImagen;

            FileUF_Archivos_Update.Reset();
            Modal_EditaObjeto.Show();
        }

        [DirectMethod]
        public void Eliminar_Event(string id)
        {
            X.Msg.Confirm("Eliminar Objeto", "¿Esta seguro que desea eliminar el Objeto?",
                new MessageBoxButtonsConfig
                {
                    Yes = new MessageBoxButtonConfig
                    {
                        Handler = "AdminObjeto.Elimina("+id+")",
                        Text = "Si"
                    },
                    No = new MessageBoxButtonConfig
                    {
                        Handler = "",
                        Text = "No"
                    }
                }).Show();
        }

        /// <summary>
        /// Controla el evento clic al botón Eliminar
        /// </summary>
        [DirectMethod(Namespace = "AdminObjeto")]
        public void Elimina(string id)
        {
            ObjetoPrograma objetoPrograma = new ObjetoPrograma();
            objetoPrograma.ID_ProgramaObjeto = Convert.ToInt32(id);
            objetoPrograma.Activo = 0;
            objetoPrograma.ID_Entidad = 0;
            objetoPrograma.ID_Programa = 0;
            objetoPrograma.ID_TipoEntidad = 0;
            objetoPrograma.ID_TipoObjeto = 0;
            objetoPrograma.Orden = "";
            objetoPrograma.PathImagen = "";
            objetoPrograma.URL = "";

            LNEcommercePrana.ModificaObjeto(objetoPrograma, this.Usuario);
            LlenaGridResultados();

            X.Msg.Notify("", "Objeto Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
        }

        /// <summary>
        /// Controla el evento Click al botón Obtener Archivos
        /// </summary>
        /// <param name="indexFileUpdate">Numero de index que indica el control de FileUpdate (0 - Agregar, 1 - Editar) </param>
        public string CargaArchivoFTP(int indexFileUpdate)
        {
            try
            {
                string tempPath, root;
                root = "C:\\TmpFTPFiles\\";

                if (Request.Files.Count > 0)
                {
                    LimpiaYCreaDirectorio();

                    HttpPostedFile file = Request.Files[indexFileUpdate];
                    tempPath = root + file.FileName;
                    file.SaveAs(tempPath);
            
                    SubeArchivo(file.FileName);

                    return file.FileName;
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
                throw ex;
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
        /// Sube el archivo al FTP indicado
        /// </summary>
        public void SubeArchivo(string archivo)
        {
            try
            {
                string root = "C:\\TmpFTPFiles\\";
                string Srvr = "";
                string Usr = "";
                string Pwd = "";
                string Ssl = "";
                string ClaveObjeto = "";

                List<string> files = new List<string>();
                files.Add(archivo);

                DataTable CarpetasFTP =
                       DAOPropiedad.ObtieneCarpetasFTP(this.Usuario,
                       Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                DataSet dtTiposObjetos = DAOEcommercePrana.ListaTipoObjeto(this.Usuario, false);
                foreach (DataRow row in dtTiposObjetos.Tables[0].Rows)
                {
                    if (row["ID_tipoObjeto"].ToString() == cBoxTipoObjeto.Value.ToString())
                    {
                        ClaveObjeto = row["Clave"].ToString();
                    }
                }
                //objetoPrograma.PathImagen = "/static/images/" + ClaveObjeto + "/" + ArchivoFTP;

                foreach (DataRow row in CarpetasFTP.Rows)
                {
                    if (row[1].ToString() == "FTP_SAMS2")
                    {
                        Srvr = row["Server"].ToString();
                        Usr = row["User"].ToString();
                        Pwd = row["Password"].ToString();
                        Ssl = row["SSL"].ToString();
                    }
                }

                Srvr = Srvr + "/" + ClaveObjeto;

                LNConexionFTP.SubeArchivosADirectorioFTP(Srvr, Usr, Pwd, Ssl, files, root);
 
                X.Mask.Hide();
                X.Msg.Notify("", "El archivo se subió <br /> <br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Subir Archivos", ex.Message).Show();
                throw ex;
            }
        }
    }
}