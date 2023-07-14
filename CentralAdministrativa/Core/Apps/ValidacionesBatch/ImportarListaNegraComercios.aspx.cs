using DALCentralAplicaciones.Utilidades;
using DALAutorizador.BaseDatos;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using DALAutorizador.Entidades;
using System.IO;

namespace ValidacionesBatch
{
    public partial class ImportarListaNegraComercios : DALCentralAplicaciones.PaginaBaseCAPP
    {
        public static Int64 ID_ReglaFija = 32;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    //Obtener los datos de la Regla

                    Regla laRegla = DAORegla.ObtnenRegla(ID_ReglaFija);

                    this.TxtNombreRegla.Text = laRegla.Nombre;
                    this.TxtDescripcion.Text = laRegla.Descripcion;


                    StoreCadenaComercial.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "CCM", "", -1);
                    StoreCadenaComercial.DataBind();

                }
            }
            catch (Exception)
            {
                // Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }


        protected void SeleccionarCadena_Click(object sender, EventArgs e)
        {
            try
            {

                Int32 ID_CadenaComercial = Int32.Parse(cmbCadenaComercial.Value.ToString());

                StoreListaNegraTarjeta.DataSource = DAOListaNegra.ObtenerListaNegraTarjetas(ID_CadenaComercial, "%", this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                StoreListaNegraTarjeta.DataBind();

            }
            catch (Exception)
            {

            }
        }


        protected void btnBuscarListaNegraTarjeta_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {
            }
        }





        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void AfterEditTarjeta(Int64 ID_ListaNegraMA, Int32 Accion)
        {


            Guid AppId = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            ParametroValor unParametro = new ParametroValor();
            String Observaciones;
            Boolean laRespuesta = false;

            try
            {
                //ASIGNACION DE VALORES:

                Observaciones = "MODIFICACION DE VALOR";

                laRespuesta = DAOListaNegra.ActualizaRegistroListaNegraTarjeta(ID_ListaNegraMA, Accion, Observaciones, this.Usuario, AppId);

                if (laRespuesta)
                {
                    X.Msg.Notify("Listas Negras", "Modificacion <br />  <br /> <b>  A U T O R I Z A D A  </b> <br />  <br /> ").Show();
                }
                else
                {
                    X.Msg.Notify("Listas Negras", "Modificacion <br />  <br /> <b>  D E C L I N A D A  </b> <br />  <br /> ").Show();
                }
                //DAOParametro.ActualizaParametro(unParametro, ID_Regla, ID_CadenaComercial, ID_Entidad, ID_EntidadEnTabla, Observaciones, this.Usuario, AppId);

            }

            catch (Exception)
            {
                X.Msg.Alert("Lista Negra", "Error al Actulaizar los Valores de la Lista Negra de Tarjeta").Show();
            }
            finally
            {

            }


            //this.GridPanel1.Store.Primary.CommitChanges();
        }


        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void AfterEditComercio(Int64 ID_ListaNegraComercio, Int32 Accion)
        {


            Guid AppId = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            ParametroValor unParametro = new ParametroValor();
            String Observaciones;
            Boolean laRespuesta = false;

            try
            {
                //ASIGNACION DE VALORES:

                Observaciones = "MODIFICACION DE VALOR";

                laRespuesta = DAOListaNegra.ActualizaRegistroListaNegraComercio(ID_ListaNegraComercio, Accion, Observaciones, this.Usuario, AppId);

                if (laRespuesta)
                {
                    X.Msg.Notify("Listas Negras", "Modificacion <br />  <br /> <b>  A U T O R I Z A D A  </b> <br />  <br /> ").Show();
                }
                else
                {
                    X.Msg.Notify("Listas Negras", "Modificacion <br />  <br /> <b>  D E C L I N A D A  </b> <br />  <br /> ").Show();
                }
                //DAOParametro.ActualizaParametro(unParametro, ID_Regla, ID_CadenaComercial, ID_Entidad, ID_EntidadEnTabla, Observaciones, this.Usuario, AppId);

            }

            catch (Exception)
            {
                X.Msg.Alert("Lista Negra", "Error al Actulaizar los Valores de la Lista Negra de Comercios").Show();
            }
            finally
            {

            }


            //this.GridPanel1.Store.Primary.CommitChanges();
        }


        protected void FileUploadField_FileSelected(object sender, DirectEventArgs e)
        {
            try
            {

                if (FileSelect.HasFile)
                {

                    this.FileSelect.PostedFile.SaveAs(Server.MapPath("../Apps/ValidacionesBatch/Archivos/") + Path.GetFileName(FileSelect.FileName));
                    //FileSelect..SaveAs(fullPath);


                    //Subir el archivo a la BD
                    // Int64 ID_Archivo = LNArchivo.ImportaArchivoBD(Server.MapPath("../Apps/ClubEscala/Archivos/") + Path.GetFileName(FileSelect.FileName), Int64.Parse(cmbCadenaComercial.SelectedItem.Value), this.Usuario);

                    //  LlenaDetalles(ID_Archivo);
                    String Mensaje = String.Format("Se ha Agendado procesamiento del archivo:  <br/> Archivo: <b> {0} </b> <br/> Tamaño:<b> {1} </b> bytes <br/>", Path.GetFileName(FileSelect.FileName), this.FileSelect.PostedFile.ContentLength);
                    X.Msg.Alert("Upload Exitoso", Mensaje).Show();


                }

            }
            catch (Exception err)
            {
                X.Msg.Alert("Upload Archivo", "Error al subir el Archivo:" + err.Message).Show();
            }
            finally
            {
                FormPanel1.Reset();
            }

        }

        protected void LimpiarFormulario(object sender, EventArgs e)
        {
            try
            {
                String Mensaje = String.Format("Se ha Agendado procesamiento del archivo:  <br/> Archivo: <b> {0} </b> <br/> Tamaño:<b> {1} </b> bytes <br/>");
                X.Msg.Alert("Upload Exitoso", Mensaje).Show();
            }
            catch (Exception err)
            {
                X.Msg.Alert("Guardar Operador", "Ocurrio un Error:" + err.Message).Show();
            }

        }

    }
}