using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DALCentralAplicaciones;
using DALClubEscala.BaseDatos;
using System.Configuration;
using Ext.Net;
using DALClubEscala.LogicaNegocio;
using System.Text;
using Framework;
using ClubEscala.Utilidades;
using System.IO;

namespace ClubEscala
{
    public partial class ArchivosIngresados : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    LlenaArchivos();
                }
            }
            catch (Exception)
            {
            }
        }


        protected void LlenaArchivos()
        {
            try
            {
                storeArchivo.DataSource = DAOArchivo.ListaArchivos(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                storeArchivo.DataBind();
            }
            catch (Exception)
            {
            }
        }

        protected void LlenaDetalles(Int64 ID_Archivo)
        {
            try
            {


                storeEmpleados.DataSource = DAOArchivo.ListaDetalleArchivos(ID_Archivo, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                storeEmpleados.DataBind();
            }
            catch (Exception)
            {
            }
        }

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {
                Int64 ID_Empleado = Int64.Parse(e.ExtraParams["ID_Empleado"]);
                Int64 ID_Archivo = Int64.Parse(e.ExtraParams["ID_Archivo"]);
                String mailEmpresa = (e.ExtraParams["EmailEmpresarial"]);
                String mailPersona = (e.ExtraParams["EmailPersonal"]);

                String mailAEnviar = mailPersona.Trim().Length == 0 ? mailEmpresa : mailPersona;
                
                String EjecutarComando = (String)e.ExtraParams["Comando"];

               
                //Solicitar una Confirmacion
                //X.Msg.Confirm("Confirm", "¿Estas seguro de realizar la Accion: <b>" + EjecutarComando + "</b> a la Ficha Numero:<b>" + laFichaSeleccionada.ID_FichaDeposito + "</b> de un Importe de " + String.Format("{0:C}", laFichaSeleccionada.Importe) + "?").Show();


                switch (EjecutarComando)
                {
                    case "Reintentar":
                        int resp=LNEmpleado.ProcesarEmpleadoPendientes(ID_Empleado, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), this.Usuario,true);
                        if (resp != 0)
                        {
                            X.Msg.Notify("Respuesta", "Resultados de la Operación <br />  Codigo Respuesta: <b>" + resp + "</b>").Show();
                            X.Msg.Notify("Respuesta", EjecutarComando + " <br />  <br /> <b> D E C L I N A D A  </b> <br />  <br /> ").Show();
                        }
                        else
                        {
                           
                           // X.Msg.Notify("Respuesta", "Resultados de la Operación <br />  Codigo Respuesta: <b>" + laRespuesta.CodigoRespuesta + "</b> <br /> Numero Autorización: <b>" + laRespuesta.Autorizacion + "</b> <br />").Show();
                            X.Msg.Notify("Respuesta", EjecutarComando + " <br />  <br /> <b> A U T O R I Z A D A </b> <br />  <br /> ").Show();
                            LNArchivo.ActualizaEstatusArchivo(ID_Archivo, Utilidades.EstatusArchivo.ParcialmenteProcesado, this.Usuario);
                        }
                        break;
                    case "CacelarProc":
                        //LNEmpleado.ProcesarEmpleadoPendientes(ID_Empleado, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), this.Usuario);
                        LNArchivo.ActualizaEstatusArchivo(ID_Archivo, Utilidades.EstatusArchivo.NoProcesar, this.Usuario);

                        LlenaArchivos();
                        break;
                }

                

                Int64 ID_Archivo2 = Int64.Parse(Session["ID_Archivo"].ToString());

                LlenaDetalles(ID_Archivo2);

            }
           
            catch (Exception err)
            {
                Loguear.Error(err,"");
                X.Msg.Alert("Procesamiento de Empleado", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }


        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {

            Panel2.Collapsed = true;
            GridPanel1.GetStore().RemoveAll();
        }



    
        protected void GridEmpleados_DblClik(object sender, DirectEventArgs e)
        {
            try
            {
                String unvalor = e.ExtraParams["ID_Archivo"];

                Session.Add("ID_Archivo", unvalor);
                Int64 ID_Archivo = Int64.Parse(unvalor);

                LlenaDetalles(ID_Archivo);

                Panel2.Collapsed = false;

            }
            catch (Exception)
            {
            }


        }

        protected void RefreshArchivos(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                LlenaArchivos();
            }
            catch (Exception)
            {
               
            }


        }

        protected void RefreshDetalles(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                LlenaDetalles(Int64.Parse(Session["ID_Archivo"].ToString()));
            }
            catch (Exception)
            {

            }


        }


    }
}