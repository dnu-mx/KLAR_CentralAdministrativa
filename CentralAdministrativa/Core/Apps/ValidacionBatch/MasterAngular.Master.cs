using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DALCentralAplicaciones;
using ValidacionBatch.Controllers.Extensions;

namespace ValidacionBatch
{
    public partial class MasterAngular : MasterPaginaBaseCAPP
    {
        public string Token = "..";

        public string zurl = HttpContext.Current.Request.Url.AbsoluteUri;
        // http://localhost:1302/TESTERS/Default6.aspx

        public string zpath = HttpContext.Current.Request.Url.AbsolutePath;
        // /TESTERS/Default6.aspx

        public string zhost = HttpContext.Current.Request.Url.Host;
         
        protected void Page_Load(object sender, EventArgs e)
        {

            var user = new UserWebApi(this.Usuario);

            ApiControllerExtension.AddUser(user);

            Token = user.Token;
        }
    }
}