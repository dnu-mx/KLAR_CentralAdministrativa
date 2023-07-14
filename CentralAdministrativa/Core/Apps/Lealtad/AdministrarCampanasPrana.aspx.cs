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
    public partial class AdministrarCampanasPrana : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Administrar las Campañas y Promociones para Prana
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
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }
       
        /// <summary>
        /// Controla el evento Click al botón Crear Campaña
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddCampana_Click(object sender, EventArgs e)
        {
            try
            {
                string ArchivoFTP = CargaArchivoFTP(1, txtClaveCampana.Text);

                Campana campana = new Campana();

                campana.ClaveCampana = txtClaveCampana.Text;
                campana.NombreComercial = txtCampana.Text;
                campana.ID_Programa = Convert.ToInt32(cBoxProgramas.SelectedItem.Value);
                campana.Activo = 1;
                campana.path = "/static/images/campanas/" + ArchivoFTP;

                DataTable dtNuevaCampana = LNEcommercePrana.CreaNuevaCampana(campana, this.Usuario);

                string msj = dtNuevaCampana.Rows[0]["Mensaje"].ToString();
                int idNuevaCampana = Convert.ToInt32(dtNuevaCampana.Rows[0]["IdNuevaCampana"]);

                if (idNuevaCampana == -1)
                {
                    X.Msg.Alert("Nueva Campaña", msj).Show();
                }
                else
                {
                    WdwNC_Camp.Hide();

                    this.txtClaveCamp.Text = txtClaveCampana.Text;
                    LlenaGridResultados();

                    X.Msg.Alert("Nueva Campaña", "<br />" + msj + "<br /> <b> E X I T O S A M E N T E </b> <br /> ",
                        new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "AdminCampana.CargaNuevaCampana()",
                                Text = "Aceptar"
                            }
                        }).Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nueva Campaña", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Nueva Campaña", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento clic al botón Aceptar al mensaje informativo de creación
        /// de cadena exitosa
        /// </summary>
        [DirectMethod(Namespace = "AdminCampana")]
        public void CargaNuevaCampana()
        {
            RowSelectionModel rsm = GridResultados.GetSelectionModel() as RowSelectionModel;
            rsm.SelectedRows.Add(new SelectedRow(0));
            rsm.UpdateSelection();

            GridResultados.FireEvent("RowClick");
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del panel izquierdo, limpiando los controles,
        /// páneles y grids asociados a alguna previa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarIzq_Click(object sender, EventArgs e)
        {
            LimpiaVentanasAltaCampana();

            this.txtClaveCamp.Reset();
            this.txtNombreCamp.Reset();
            StoreCampanas.RemoveAll();

            LimpiaSeleccionPrevia();

            PanelCentralCamp.Disabled = true;
        }

        /// <summary>
        /// Limpia los controles de las ventanas pop up para añadir nuevas campañas
        /// </summary>
        protected void LimpiaVentanasAltaCampana()
        {
            FormPanelNC_Camp.Reset();
            this.txtClaveCampana.Reset();
            this.txtCampana.Reset();
            this.cBoxProgramas.Reset();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de
        /// una promoción en el Grid de Resultados
        /// </summary>
        protected void LimpiaSeleccionPrevia()
        {
            PanelCentralCamp.SetTitle("_");

            FormPanelInfoAdCampana.Reset();
            btnGuardaInfoAdCampana.Disabled = true;

            this.StorePromociones.RemoveAll();
            this.txtClavePromo.Reset();
            txtNombrePromo.Reset();

            FormPanelPromociones.Reset();

            FormPanelPromocionesAsignadas.Reset();
            FormPanelPromocionesAsignadas.SetTitle("Promociones Asignadas");
            
            FormPanelInfoAdCampana.Show();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel izquierdo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarCamp_Click(object sender, EventArgs e)
        {
            LlenaGridResultados();
        }

        /// <summary>
        /// Llena el grid de resultados de campañas con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            try
            {
                DataSet dsCampanas = DAOEcommercePrana.ObtieneCampanasPorClaveONombre(
                    this.txtClaveCamp.Text, this.txtNombreCamp.Text, this.Usuario);

                if (dsCampanas.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Campañas", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreCampanas.DataSource = dsCampanas;
                    StoreCampanas.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Campañas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Campañas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowResultados_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdCampana = 0;
                string ClaveCampana = "", Nombre = "";

                LimpiaSeleccionPrevia();

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] cadena = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in cadena[0])
                {
                    switch (column.Key)
                    {
                        case "Id_Campana": IdCampana = int.Parse(column.Value); break;
                        case "ClaveCampana": ClaveCampana = column.Value; break;
                        case "Nombre": Nombre = column.Value; break;
                        default:
                            break;
                    }
                }

                hdnIdCamp.Value = IdCampana;
                LlenaFormPanelInfoAd(ClaveCampana);
                btnGuardaInfoAdCampana.Disabled = false;

                /*Promos asignadas*/
                ActualizaPanelPromociones(0);

                PanelCentralCamp.SetTitle(ClaveCampana + " - " + Nombre);
                PanelCentralCamp.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Campañas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                X.Msg.Alert("Campañas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el panel de Información Adicional con los datos consultados a base de datos
        /// </summary>
        /// <param name="ClaveCampana">Clave de la campaña</param>
        protected void LlenaFormPanelInfoAd(string ClaveCampana)
        {
            try
            {
                DataSet Campana = DAOEcommercePrana.ObtieneCampanasPorClaveONombre(ClaveCampana, "", this.Usuario);

                txtClaveCampAd.Text = ClaveCampana;
                hdnlblClaveCampAd.Text = ClaveCampana;
                txtNombreCampAd.Text = Campana.Tables[0].Rows[0]["Nombre"].ToString();
                cBoxProgramasAd.Value = Campana.Tables[0].Rows[0]["Id_Programa"].ToString();
                chkActivo.Value = Convert.ToInt16(Campana.Tables[0].Rows[0]["Activa"]);
                lblFile_Update.Text = Campana.Tables[0].Rows[0]["PathImagen"].ToString();
                hdnlblFile_Update.Text = Campana.Tables[0].Rows[0]["PathImagen"].ToString();
                FileUF_Archivos_Update.Reset();

                btnGuardaInfoAdCampana.Disabled = false;
            }
            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Información Adicional", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Información Adicional", ex.Message).Show();
            }
        }
        /// <summary>
        /// Controla el evento Click al botón Guardar del panel de Información Adicional, llamando
        /// a la actualización de datos de la campaña en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarInfoAd_Click(object sender, EventArgs e)
        {
            try
            {
                Campana campana = new Campana();
                string ArchivoFTP = "";

                if (FileUF_Archivos_Update.HasFile)
                {
                    ArchivoFTP = CargaArchivoFTP(0, hdnlblClaveCampAd.Text);
                }

                campana.ID_Campana = Convert.ToInt32(hdnIdCamp.Value);
                campana.NombreComercial = this.txtNombreCampAd.Text;
                campana.ID_Programa = Convert.ToInt32(this.cBoxProgramasAd.SelectedItem.Value);
                campana.Activo = Convert.ToInt16(this.chkActivo.Value);

                if (ArchivoFTP == "")
                {
                    campana.path = hdnlblFile_Update.Text;
                }
                else
                {
                    campana.path = "/static/images/campanas/" + ArchivoFTP;
                }

                LNEcommercePrana.ModificaCampana(campana, this.Usuario);
                lblFile_Update.Text = campana.path;

                /*Limpia Promos disponibles*/
                this.StorePromociones.RemoveAll();

                /*Limpia Promos asignadas*/
                this.StorePromocionesAsignadas.RemoveAll();

                X.Msg.Notify("", "Campaña Actualizada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Campaña", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Actualización de Campaña", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el panel de PROMOCIONES DISPONIBLES y ASIGNADAS de la campaña seleccionada
        /// </summary>
        protected void ActualizaPanelPromociones(int Disponibles, string ClavePromocion = "", string Descripcion = "")
        {
            try
            {
                DataSet dsPromociones = DAOEcommercePrana.ObtienePromocionesPorCampana(Disponibles, 
                    Convert.ToInt32(hdnIdCamp.Value), this.Usuario, ClavePromocion, Descripcion);

                if (Disponibles == 1)
                {
                    if (dsPromociones.Tables[0].Rows.Count == 0)
                    {
                        X.Msg.Alert("Promociones", "No existen coincidencias con los datos solicitados").Show();
                    }
                    else
                    {
                        StorePromociones.DataSource = dsPromociones;
                        StorePromociones.DataBind();
                    }
                    
                } else
                {

                    StorePromocionesAsignadas.DataSource = dsPromociones;
                    StorePromocionesAsignadas.DataBind();
                }
                
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Promociones", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Promociones", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel de Promociones Disponibles
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarPromo_Click(object sender, EventArgs e)
        {
            try
            {
                this.StorePromociones.RemoveAll();
                this.StorePromocionesAsignadas.RemoveAll();
                ActualizaPanelPromociones(1, txtClavePromo.Text, txtNombrePromo.Text);
                ActualizaPanelPromociones(0);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Sucursales", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Sucursales", ex.Message).Show();
            }
        }
        
        /// <summary>
        /// Controla el evento Click al botón Guardar del panel de Promociones llamando
        /// a la actualización de datos de la misma en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarPromos_Click(object sender, DirectEventArgs e)
        {
            try
            {
                string jsonValues = e.ExtraParams["Promociones"];
                List<Dictionary<string, string>> promociones = JSON.Deserialize<List<Dictionary<string, string>>>(jsonValues);

                string PromocionesEnviar = "";
                string coma = "";

                /*recorrer para concatenar el id de cada promoción*/
                foreach (var promocion in promociones)
                {
                    PromocionesEnviar = PromocionesEnviar + coma + promocion["ID_Promocion"];
                    coma = ",";
                }

                LNEcommercePrana.AplicaPromociones(Convert.ToInt32(hdnIdCamp.Value), PromocionesEnviar, this.Usuario);
                X.Msg.Notify("", "Promociones Asignadas" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (Exception)
            {
                X.Msg.Notify("Asignación de Promoción", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Asignar promociones para agregarla al grid GridPromocionesAsignadas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void AgregarPromocion(object sender, DirectEventArgs e)
        {
            try
            {
                int ID_Promocion = int.Parse(e.ExtraParams["ID_Promocion"]);
                string ClavePromocion = e.ExtraParams["ClavePromocion"].ToString();
                string Descripcion = e.ExtraParams["Descripcion"].ToString();

                string jsonValues = e.ExtraParams["Promociones"];
                List<Dictionary<string, string>> promociones = JSON.Deserialize<List<Dictionary<string, string>>>(jsonValues);

                int i = 0;

                /*recorrer para saber el id row de la promoción eliminada*/
                foreach (var promocion in promociones)
                {
                    if (promocion["ClavePromocion"] == ClavePromocion)
                    {
                        break;
                    }
                    i++;
                }

                /*Elimina registro del grid de disponibles*/
                this.StorePromociones.RemoveRecord(i);

                /*Agrega registro al grid de promociones asignadas*/
                var rowPromo = new {ID_Promocion = ID_Promocion, ClavePromocion = ClavePromocion, Descripción = Descripcion };
                this.GridPromocionesAsignadas.AddRecord(rowPromo);
            }

            catch (Exception)
            {
                X.Msg.Notify("Asignación de Promoción", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Asignar promociones para eliminarla del grid GridPromocionesAsignadas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EliminarPromocion(object sender, DirectEventArgs e)
        {
            try
            {
                int ID_Promocion = int.Parse(e.ExtraParams["ID_Promocion"]);
                string ClavePromocion = e.ExtraParams["ClavePromocion"].ToString();
                string Descripcion = e.ExtraParams["Descripcion"].ToString();

                string jsonValues = e.ExtraParams["Promociones"];
                List<Dictionary<string, string>> promociones = JSON.Deserialize<List<Dictionary<string, string>>>(jsonValues);

                int i = 0;

                /*recorrer para saber el id row de la promoción eliminada*/
                foreach (var promocion in promociones)
                {
                    if (promocion["ClavePromocion"] == ClavePromocion)
                    {
                        break;
                    }
                    i++;
                }

                /*Elimina registro del grid de disponibles*/
                this.StorePromocionesAsignadas.RemoveRecord(i);

                /*Agrega registro al grid de promociones asignadas*/
                var rowPromo = new { Id_Promocion = ID_Promocion, ClavePromocion = ClavePromocion, Descripción = Descripcion };
                this.GridPromociones.AddRecord(rowPromo);
            }

            catch (Exception)
            {
                X.Msg.Notify("Asignación de Promoción", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Obtener Archivos
        /// </summary>
        /// <param name="indexFileUpdate">Numero de index que indica el control de FileUpdate (0 - Agregar, 1 - Editar) </param>
        public string CargaArchivoFTP(int indexFileUpdate, string nameFileAs)
        {
            try
            {
                string tempPath, root;
                
                root = "C:\\TmpFTPFiles\\";

                if (Request.Files.Count > 0)
                {
                    LimpiaYCreaDirectorio();

                    HttpPostedFile file = Request.Files[indexFileUpdate];
                    nameFileAs = nameFileAs + "." + file.FileName.Substring(file.FileName.IndexOf(".") + 1);

                    tempPath = root + nameFileAs;
                    file.SaveAs(tempPath);

                    SubeArchivo(nameFileAs);

                    return nameFileAs;
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

                List<string> files = new List<string>();
                files.Add(archivo);

                DataTable CarpetasFTP =
                       DAOPropiedad.ObtieneCarpetasFTP(this.Usuario,
                       Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

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

                Srvr = Srvr + "/campanas/";

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