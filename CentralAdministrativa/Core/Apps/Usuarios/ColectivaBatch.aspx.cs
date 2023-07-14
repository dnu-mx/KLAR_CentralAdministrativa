using DALAutorizador.Utilidades;
using Ext.Net;
using System;

namespace Usuarios
{
    public partial class ColectivaBatch : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
               
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        protected void Button_Click(object sender, DirectEventArgs e)
        {
            X.Msg.Alert("DirectEvent", string.Format("Item - {0}", e.ExtraParams["Item"])).Show();
        }

        protected void RowSelect(object sender, DirectEventArgs e)
        {
            //EastPanel.Collapsed = false;


        }

        protected void UploadClick(object sender, DirectEventArgs e)
        {
            string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";

            if (this.BasicField.HasFile)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Title = "Success",
                    Message = string.Format(tpl, this.BasicField.PostedFile.FileName, this.BasicField.PostedFile.ContentLength)
                });
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "Fail",
                    Message = "No file uploaded"
                });
            }
        }

    }
}