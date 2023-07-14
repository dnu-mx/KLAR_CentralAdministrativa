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


namespace ValidacionesBatch
{
    public partial class OperacionesListaNegraComercios : DALCentralAplicaciones.PaginaBaseCAPP
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

                StoreListaNegraComercio.DataSource = DAOListaNegra.ObtenerListaNegraComercios(ID_CadenaComercial, txtComercio.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));// DAOColectiva.ListaSaldosColectiva(ID_colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                StoreListaNegraComercio.DataBind();

            }
            catch (Exception)
            {

            }
        }





        protected void btnBuscarListaNegraComercios_Click(object sender, EventArgs e)
        {
            try
            {
                Int32 ID_CadenaComercial = Int32.Parse(cmbCadenaComercial.Value.ToString());

                StoreListaNegraComercio.DataSource = DAOListaNegra.ObtenerListaNegraComercios(ID_CadenaComercial, txtComercio.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));// DAOColectiva.ListaSaldosColectiva(ID_colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                StoreListaNegraComercio.DataBind();
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



    }
}