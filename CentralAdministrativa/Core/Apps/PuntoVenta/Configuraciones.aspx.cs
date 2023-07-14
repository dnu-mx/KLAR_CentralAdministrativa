using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Ext.Net;
using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Entidades;
using System.Configuration;
using System.Web.Security;
using Interfases;

namespace TpvWeb
{
    public partial class Configuraciones : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                  

                    PreparaGripPropiedades();
                }
            }
            catch (Exception )
            {
               // Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
            
        }

        protected void PreparaGripPropiedades()
        {
            try
            {
                foreach (Propiedad unaProp in LNPropiedad.ObtieneParametros(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString())))
                {
                    PropertyGridParameter GridProp = new PropertyGridParameter(unaProp.Nombre, unaProp.Valor);
                    GridProp.DisplayName = unaProp.ValueDescription;
                    Propiedades.AddProperty(GridProp);
                }
            }
            catch (Exception )
            {
            }
        }

        protected void Button1_Click(object sender, DirectEventArgs e)
        {
            try
            {
                List<Propiedad> losCambios = new List<Propiedad>();


                //Obtiene las propiedades que cambiaron
                foreach (PropertyGridParameter param in this.Propiedades.Source)
                {
                    if (param.IsChanged)
                    {
                        Propiedad unaProp = new Propiedad(param.Name, param.Value.ToString(), "", new Guid(), Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "");
                        losCambios.Add(unaProp);
                    }
                }

                //Guardar Valores
                LNPropiedad.ModificaParametros(losCambios, this.Usuario);

                ValoresInicial.InicializarContexto();


                X.Msg.Notify("Configuración", "Modificación de Configuración <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();


            }
            catch (Exception )
            {
                X.Msg.Notify("Configuración", "Modificación de Configuración <br /><br />  <b> D E C L I N A D O </b> <br />  <br /> ").Show();
            }

        }

       


    }
}