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
    public partial class OperacionesMayoresPorcentajeSaldo : DALCentralAplicaciones.PaginaBaseCAPP
    {
        public static Int64 ID_ReglaFija = 33;
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

                StoreValoresMA.RemoveAll();
                StoreValoresGrupoMA.RemoveAll();
                StoreValoresGrupoCuenta.RemoveAll();
                StoreValoresTipoCuenta.RemoveAll();


                StoreValoresRegla.DataSource = DAOParametro.ObtieneParametrosRegla(ID_CadenaComercial, ID_ReglaFija, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                StoreValoresRegla.DataBind();

            }
            catch (Exception)
            {

            }
        }

        protected void btnBuscarTipoCuenta_Click(object sender, EventArgs e)
        {
            try
            {
                gridTipoCuenta.GetStore().DataSource = DAOCatalogos.BuscarTipoCuenta(txtClaveTipoCuenta.Text, txtDescTipoCuenta.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));// DAOColectiva.ListaSaldosColectiva(ID_colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                gridTipoCuenta.GetStore().DataBind();
            }
            catch (Exception)
            {
            }
        }


        protected void EjecutarAccionValorRegla(object sender, DirectEventArgs e)
        {
            try
            {
                Int64 ID_ValorReglaValor = Int64.Parse(e.ExtraParams["ID_ValorReglaValor"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                switch (EjecutarComando)
                {

                    case "EliminarValor":

                        if (DAORegla.EliminarValorDePrioridad(ID_ValorReglaValor, this.Usuario))
                        {
                            X.Msg.Notify("Eliminacion de Parámetro", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();
                        }
                        else
                        {
                            X.Msg.Notify("Eliminacion de Parámetro", "Comando ejecutado con <br />  <br /> <b> E R R O R </b> <br />  <br /> ").Show();
                        }


                        return;
                }

                EnlazaDatos();


            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
            finally
            {
                EnlazaDatos();
            }
        }

        protected void btnBuscarGrupoCuenta_Click(object sender, EventArgs e)
        {
            try
            {
                gridGrupoCuenta.GetStore().DataSource = DAOCatalogos.BuscarGrupoCuenta(txtClaveGrupoCuenta.Text, txtDescripcionGrupoCuenta.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));// DAOColectiva.ListaSaldosColectiva(ID_colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                gridGrupoCuenta.GetStore().DataBind();
            }
            catch (Exception)
            {
            }
        }

        protected void btnBuscarGrupoMA_Click(object sender, EventArgs e)
        {
            try
            {
                gridGrupoMA.GetStore().DataSource = DAOCatalogos.BuscarGrupoTarjeta(txtClaveGrupoMA.Text, txtDescripcionGrupoMA.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));// DAOColectiva.ListaSaldosColectiva(ID_colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                gridGrupoMA.GetStore().DataBind();
            }
            catch (Exception)
            {
            }
        }


        protected void EnlazaDatos()
        {
            //Int64 ID_CadenaComercial = (Int64)HttpContext.Current.Session["ID"];
            try
            {

                Int64 ID_CadenaComercial = 0;


                //vALIDA QUE TENGA UNA SELECCION EL COMBO DE CADENA COMERCIAL.

                try
                {
                    ID_CadenaComercial = Int64.Parse(cmbCadenaComercial.Value.ToString());

                }
                catch (Exception)
                {
                    ID_CadenaComercial = 0;
                }



                Int64 ID = (Int64)HttpContext.Current.Session["ID"];
                Int64 ID_Entidad = (Int64)HttpContext.Current.Session["ID_Entidad"];

                String Titulo = (String)HttpContext.Current.Session["Titulo"];

                if ((ID_CadenaComercial == 0) & (ID_Entidad != 6))
                {
                    X.Msg.Notify("PPF.Edición Parametros", "Por favor, Seleccionar una Cadena Comercial ").Show();
                    return;
                }

                switch (ID_Entidad)
                {
                    case 2:



                        Panel3.Title = "Valores para [" + Titulo + "]";
                        StoreValoresTipoCuenta.DataSource = DAOParametro.ObtieneParametrosTipoCuenta(ID_CadenaComercial, ID_ReglaFija, ID, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                        StoreValoresTipoCuenta.DataBind();

                        break;
                    case 3:
                        Panel312.Title = "Valores para [" + Titulo + "]";
                        StoreValoresGrupoCuenta.DataSource = DAOParametro.ObtieneParametrosGrupoCuenta(ID_CadenaComercial, ID_ReglaFija, ID, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                        StoreValoresGrupoCuenta.DataBind();

                        break;
                    case 4:

                        Panel7.Title = "Valores para [" + Titulo + "]";
                        StoreValoresGrupoMA.DataSource = DAOParametro.ObtieneParametrosGrupoMedioAcceso(ID_CadenaComercial, ID_ReglaFija, ID, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                        StoreValoresGrupoMA.DataBind();

                        break;
                    case 5:

                        Panel9.Title = "Valores para [" + Titulo + "]";
                        StoreValoresMA.DataSource = DAOParametro.ObtieneParametrosMedioAcceso(ID_CadenaComercial, ID_ReglaFija, ID, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                        StoreValoresMA.DataBind();

                        break;
                    case 6:

                        Panel9.Title = "Valores para [" + Titulo + "]";
                        StoreValoresMA.RemoveAll();


                        StoreValoresGrupoMA.RemoveAll();

                        StoreValoresGrupoCuenta.RemoveAll();

                        StoreValoresTipoCuenta.RemoveAll();

                        break;

                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {

                Int64 ID_CadenaComercial = 0;


                //vALIDA QUE TENGA UNA SELECCION EL COMBO DE CADENA COMERCIAL.

                try
                {
                    ID_CadenaComercial = Int64.Parse(cmbCadenaComercial.Value.ToString());

                }
                catch (Exception)
                {
                    ID_CadenaComercial = 0;
                }




                Int64 ID = Int64.Parse(e.ExtraParams["ID"]);
                Int64 ID_Entidad = Int64.Parse(e.ExtraParams["ID_Entidad"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];
                String Titulo = (String)e.ExtraParams["Titulo"];


                HttpContext.Current.Session.Add("ID", ID);
                HttpContext.Current.Session.Add("ID_Entidad", ID_Entidad);
                HttpContext.Current.Session.Add("ID_CadenaComercial", ID_CadenaComercial);
                HttpContext.Current.Session.Add("Titulo", Titulo);



                EnlazaDatos();


            }
            catch (Interfases.Exceptiones.CAppException err)
            {
                X.Msg.Alert("PPF.Edición Parametros", err.Mensaje()).Show();
                //Loguear.Error(new Exception(err.Mensaje), this.Usuario.ClaveUsuario);
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                //  X.Msg.Alert("Facturación", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
                X.Msg.Notify("PPF.Edición Parametros", err.Message).Show();

            }

        }

        protected void btnBuscarTarjeta_Click(object sender, EventArgs e)
        {
            try
            {
                gridTarjeta.GetStore().DataSource = DAOCatalogos.BuscarTarjeta(txtClaveMA.Text, txtDescripcionMA.Text, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));// DAOColectiva.ListaSaldosColectiva(ID_colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                gridTarjeta.GetStore().DataBind();
            }
            catch (Exception)
            {
            }
        }

        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void AfterEdit(Int64 ID_ReglaValorRegla, Int32 ID_Parametro, String ValorAlerta, String ValorBloquea, String ValorCancela, Int32 ID_Entidad, Int64 ID_EntidadEnTabla, Int64 ID_CadenaComercial, Int32 ID_Regla)
        {


            Guid AppId = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            ParametroValor unParametro = new ParametroValor();
            String Observaciones;
            Boolean laRespuesta = false;

            try
            {
                //ASIGNACION DE VALORES:
                unParametro.ValorAviso = ValorAlerta;
                unParametro.ValorBloquea = ValorBloquea;
                unParametro.ValorCancela = ValorCancela;
                unParametro.ID_ValordelParametro = ID_ReglaValorRegla;
                unParametro.ID_Parametro = ID_Parametro;
                int laValidacion = 0;

                if ((ValorAlerta.Trim().Length == 0) || ValorAlerta.Trim().Equals("-1", StringComparison.CurrentCultureIgnoreCase))
                {
                    laValidacion++;
                }

                if ((ValorBloquea.Trim().Length == 0) || ValorBloquea.Trim().Equals("-1", StringComparison.CurrentCultureIgnoreCase))
                {
                    laValidacion++;
                }

                if ((ValorCancela.Trim().Length == 0) || ValorCancela.Trim().Equals("-1", StringComparison.CurrentCultureIgnoreCase))
                {
                    laValidacion++;
                }

                if (laValidacion > 0)
                {

                    X.Msg.Notify("Prevencion de Fraudes", "Modificacion <br />  <br /> <b>  D E C L I N A D A  </b> <br />  <br /> ").Show();
                    X.Msg.Notify("Prevencion de Fraudes", " <br />   <b>Se han ingresado datos inválidos<br />  o no estan completos <br /> ").Show();
                    // EnlazaDatos();
                    EnlazaDatos();
                    return;
                }


                if (ID_ReglaValorRegla == 0)
                {
                    Observaciones = "ASIGNACION DE VALOR";
                }
                else
                {
                    Observaciones = "MODIFICACION DE VALOR";
                }


                laRespuesta = DAOParametro.ActualizaParametro(unParametro, ID_Regla, ID_CadenaComercial, ID_Entidad, ID_EntidadEnTabla, Observaciones, this.Usuario, AppId);

                if (laRespuesta)
                {
                    X.Msg.Notify("Prevencion de Fraudes", "Modificacion <br />  <br /> <b>  A U T O R I Z A D A  </b> <br />  <br /> ").Show();
                }
                else
                {
                    X.Msg.Notify("Prevencion de Fraudes", "Modificacion <br />  <br /> <b>  D E C L I N A D A  </b> <br />  <br /> ").Show();
                }

                EnlazaDatos();

            }

            catch (Exception)
            {
                X.Msg.Alert("Paramentros de Referencia", "Error al Actulaizar los Valores de Referencia").Show();
            }
            finally
            {

            }


            //this.GridPanel1.Store.Primary.CommitChanges();
        }

        protected void Unnamed_Event(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Session.Add("ID", Int64.Parse("0"));
                HttpContext.Current.Session.Add("ID_Entidad", Int64.Parse("6"));
                HttpContext.Current.Session.Add("ID_CadenaComercial", Int64.Parse("0"));


                EnlazaDatos();
            }
            catch (Exception)
            {
            }
        }



    }
}