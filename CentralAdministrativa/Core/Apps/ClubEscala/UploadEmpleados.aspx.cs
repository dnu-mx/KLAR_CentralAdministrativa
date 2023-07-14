using DALAutorizador.BaseDatos;
using DALCentralAplicaciones;
using DALClubEscala.LogicaNegocio;
using Ext.Net;
using System;
using System.Configuration;
using System.IO;

namespace ClubEscala
{
    public partial class UploadEmpleados : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    LlenaCadenaComercial();
                }
            }
            catch (Exception)
            {
            }

        }

        protected void LlenaCadenaComercial()
        {
            try
            {


                storeCadenaComercial.DataSource = DAOCatalogos.ListaCadenasComerciales(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                storeCadenaComercial.DataBind();
            }
            catch (Exception)
            {
            }
        }

        protected void LlenaDetalles(Int64 ID_Archivo)
        {
            try
            {


                storeEmpleados.DataSource = DALClubEscala.BaseDatos.DAOArchivo.ListaDetalleArchivos(ID_Archivo,this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                storeEmpleados.DataBind();
            }
            catch (Exception)
            {
            }
        }

        protected void FileUploadField_FileSelected(object sender, DirectEventArgs e)
        {
            try
            {

                if (FileSelect.HasFile)
                {

                    this.FileSelect.PostedFile.SaveAs(Server.MapPath("../Apps/ClubEscala/Archivos/") + Path.GetFileName(FileSelect.FileName));
                    //FileSelect..SaveAs(fullPath);


                    //Subir el archivo a la BD
                   Int64 ID_Archivo= LNArchivo.ImportaArchivoBD(Server.MapPath("../Apps/ClubEscala/Archivos/") + Path.GetFileName(FileSelect.FileName), Int64.Parse(cmbCadenaComercial.SelectedItem.Value), this.Usuario);

                   LlenaDetalles(ID_Archivo);
                   String Mensaje = String.Format("Se ha Agendado procesamiento del archivo:  <br/> Archivo: <b> {0} </b> <br/> Tamaño:<b> {1} </b> bytes <br/>", Path.GetFileName(FileSelect.FileName), this.FileSelect.PostedFile.ContentLength);
                   X.Msg.Alert("Upload Exitoso",Mensaje).Show();
                    

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