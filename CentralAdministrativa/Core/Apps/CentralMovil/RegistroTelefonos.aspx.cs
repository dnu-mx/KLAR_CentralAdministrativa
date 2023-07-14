using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using DALCentralAplicaciones;
using System.Configuration;
using DALAutorizador.Utilidades;
using Interfases.Exceptiones;
using DALAutorizador.BaseDatos;


namespace CentralMovil
{
    public partial class RegistroTelefonos : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    this.Store1.DataSource = DALAutorizador.BaseDatos.DAOCMTelefono.ListaTelefonosSupervisores(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.Store1.DataBind();

                    Store3.DataSource = DALAutorizador.BaseDatos.DAOColectiva.ListaCadenasComerciales(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    Store3.DataBind();
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

        }

        protected void Store2_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            string id = e.Parameters["ID_Telefono"];
           // string elSupervisor = e.Parameters["TelefonoSupervisor"];

            try
            {



                pnlSouth.SetTitle("Operadores del Supervisor con Identificador: [" + id + "]" );


                this.Store2.DataSource = DALAutorizador.BaseDatos.DAOCMTelefono.ListaTelefonosOperadoresdeSupervisores( Int64.Parse(id),this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                this.Store2.DataBind();


                HttpContext.Current.Session.Add("ID_TelefonoSuperVisor",Int64.Parse(id));
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

          //  this.LinqDataSource2.WhereParameters["SupplierID"].DefaultValue = id ?? "-1";

          //  this.Store2.DataBind();
        }

        protected void Store1_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
          
            try
            {
                this.Store1.DataSource = DALAutorizador.BaseDatos.DAOCMTelefono.ListaTelefonosSupervisores(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                this.Store1.DataBind();

                Store3.DataSource = DALAutorizador.BaseDatos.DAOColectiva.ListaCadenasComerciales(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                Store3.DataBind();
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

          
        }



        [DirectMethod(Namespace = "CentralMovil")]
        public void AgregarTelefono(Int64 ID_Telefono,  String Telefono, Int64  ID_CadenaComercial)
        {
           

            Guid AppId = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());

            try
            {

                if (ID_CadenaComercial == 0 && ID_Telefono == 0)
                {
                    ID_Telefono = (Int64)HttpContext.Current.Session["ID_TelefonoSuperVisor"];
                }


                DAOCMTelefono.AgregarTelefono(ID_Telefono,  Telefono, ID_CadenaComercial, this.Usuario);

               

                if (ID_CadenaComercial == 0)
                {
                    this.Store2.DataSource = DALAutorizador.BaseDatos.DAOCMTelefono.ListaTelefonosOperadoresdeSupervisores((Int64)HttpContext.Current.Session["ID_TelefonoSuperVisor"], this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.Store2.DataBind();
                }
                else
                {
                    this.Store1.DataSource = DALAutorizador.BaseDatos.DAOCMTelefono.ListaTelefonosSupervisores(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.Store1.DataBind();
                }


                    X.Msg.Notify("Registro de Teléfono", " <br />  <br /> <b>E X I T O S O </b> <br />  <br /> ").Show();

               

            }
            catch (CAppException err)
            {
                X.Msg.Alert("Registro de Teléfono", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Registro de Teléfono", "No es posible registrar el Telefono").Show();
            }
            finally
            {
                
            }


            //this.GridPanel1.Store.Primary.CommitChanges();
        }


        //[DirectMethod(Namespace = "CentralMovil")]
        public void EliminarTelefonoSup(object sender, DirectEventArgs e)
        {

            string ID_Telefono = e.ExtraParams["ID_Telefono"];
            

            Guid AppId = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());

            try
            {


                DAOCMTelefono.EliminarTelefono(Int64.Parse(ID_Telefono), this.Usuario);


                X.Msg.Notify("Eliminar Teléfono", " <br />  <br /> <b>E X I T O S O </b> <br />  <br /> ").Show();



            }
            catch (CAppException err)
            {
                X.Msg.Alert("Eliminar Teléfono", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Eliminar Teléfono", "No es posible registrar el Telefono").Show();
            }
            finally
            {
                this.Store1.DataSource = DALAutorizador.BaseDatos.DAOCMTelefono.ListaTelefonosSupervisores(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                this.Store1.DataBind();
            }


            //this.GridPanel1.Store.Primary.CommitChanges();
        }


        public void EliminarTelefonoOper(object sender, DirectEventArgs e)
        {

            string ID_Telefono = e.ExtraParams["ID_Telefono"];


            Guid AppId = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());

            try
            {


                DAOCMTelefono.EliminarTelefono(Int64.Parse(ID_Telefono), this.Usuario);


                X.Msg.Notify("Eliminar Teléfono", " <br />  <br /> <b>E X I T O S O </b> <br />  <br /> ").Show();



            }
            catch (CAppException err)
            {
                X.Msg.Alert("Eliminar de Teléfono", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Eliminacion de Teléfono", "No es posible Borrar el Telefono").Show();
            }
            finally
            {

                if (HttpContext.Current.Session["ID_TelefonoSuperVisor"] != null)
                {
                    this.Store2.DataSource = DALAutorizador.BaseDatos.DAOCMTelefono.ListaTelefonosOperadoresdeSupervisores((Int64)HttpContext.Current.Session["ID_TelefonoSuperVisor"], this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                }
                else
                {
                    this.Store2.DataSource = DALAutorizador.BaseDatos.DAOCMTelefono.ListaTelefonosOperadoresdeSupervisores(0, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                }
                this.Store2.DataBind();

            }


            //this.GridPanel1.Store.Primary.CommitChanges();
        }


    }
}