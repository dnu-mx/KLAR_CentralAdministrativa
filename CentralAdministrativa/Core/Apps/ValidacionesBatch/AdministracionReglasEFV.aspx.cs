using DALAutorizador.BaseDatos;
using DALValidacionesBatchPPF.BaseDatos;
using DALValidacionesBatchPPF.Entidades;
using DALValidacionesBatchPPF.LogicaNegocio;
using DALValidacionesBatchPPF.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ValidacionesBatch
{
    public partial class AdministracionReglasEFV : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Administración de Reglas Efectivale
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    //Se cargan los catálogos
                    StoreReglas.DataSource = DAOEfectivaleOnline.ListaReglas(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString())); ;
                    StoreReglas.DataBind();
                    
                    DataSet dsColectivas = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "CCM", "", -1);
                    StoreCadenaComercial.DataSource = dsColectivas;
                    StoreCadenaComercial.DataBind();

                    foreach (DataRow row in dsColectivas.Tables[0].Rows)
                    {
                        if (row["ClaveColectiva"].ToString().Trim() == "TRAVEL")
                        {
                            //Se prestablece como default la empresa TRAVEL-Travel and Expenses 
                            cBoxEmpresa.SetValue(Convert.ToInt32(row["ID_Colectiva"].ToString()));
                        }
                    }

                    StoreEntidad.DataSource = DAOEfectivaleOnline.ListaEntidades(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString())); ;
                    StoreEntidad.DataBind();
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla el evento de Click al botón de Buscar del Grid Entidades
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarEntidad_Click(object sender, EventArgs e)
        {
            try
            {
                StoreResultadoEntidad.DataSource = DAOEfectivaleOnline.ListaCatalogoEntidad(
                    int.Parse(this.hdnIdEntidad.Text), this.txtClave.Text, this.txtDescripcion.Text,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreResultadoEntidad.DataBind();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Búsqueda de " + this.cBoxNivelConfig.SelectedItem.Text, ex.Message).Show();
            }
        }


        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowEntidad_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdRegistroEntidad = 0;
                this.ToolbarTarjeta.Hide();
                this.btnEstatusTarjeta.Hide();
                this.btnCerrarCaso.Hide();

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] colectiva = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in colectiva[0])
                {
                    switch (column.Key)
                    {
                        case "ID_RegistroEntidad": IdRegistroEntidad = int.Parse(column.Value); break;
                        case "ClaveEntidad": this.hdnClaveEntidad.Text = column.Value; break;
                        case "Estatus": this.hdnEstatus.Text = column.Value; break;
                        default:
                            break;
                    }
                }
                
                LlenaPanelParametros(IdRegistroEntidad);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Parámetros", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                X.Msg.Alert("Parámetros", "Ocurrió un error al consultar los parámetros").Show();
            }
        }

        /// <summary>
        /// Llena el control PanelParametros con la información obtenida de base de datos
        /// </summary>
        /// <param name="idRegistroEntidad">Identificador del registro entidad</param>
        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void LlenaPanelParametros(int idRegistroEntidad)
        {
            try
            {
                this.hdnIdRegEntidad.Text = idRegistroEntidad.ToString();

                StoreValoresParametros.RemoveAll();

                StoreValoresParametros.DataSource = DAOEfectivaleOnline.ObtieneParametrosRegla(
                    int.Parse(this.cBoxReglas.SelectedItem.Value),
                    Convert.ToInt64(this.cBoxEmpresa.SelectedItem.Value),
                    int.Parse(this.cBoxNivelConfig.SelectedItem.Value),
                    idRegistroEntidad == -1 ? int.Parse(this.cBoxReglas.SelectedItem.Value) : idRegistroEntidad,
                    this.Usuario);
                StoreValoresParametros.DataBind();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Parámetros", ex.Message).Show();
            }
        }

        /// <summary>
        /// Actualiza el(los) valor(es) del parámetro con el ID recibido
        /// </summary>
        /// <param name="ID_Parametro">Identificador dek parámetro</param>
        /// <param name="Clave">Clave del parámetro</param>
        /// <param name="ValorAlerta">Valor Alertar</param>
        /// <param name="ValorRechaza">Valor Rechazar</param>
        /// <param name="ValorBloquea">Valor Bloquear</param>
        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void ActualizaValorParametro(int ID_Parametro, string Clave, string ValorDefault, 
            string ValorAlerta, string ValorRechaza, string ValorBloquea)
        {
            try
            {
                int idRegEntidad = int.Parse(this.hdnIdRegEntidad.Text);
                ValorParametroMARegla NuevoValor = new ValorParametroMARegla();

                NuevoValor.ID_ParametroMA = ID_Parametro;
                NuevoValor.ID_Entidad = int.Parse(this.cBoxNivelConfig.SelectedItem.Value);
                NuevoValor.ID_CadenaComercial = Convert.ToInt64(this.cBoxEmpresa.SelectedItem.Value);
                NuevoValor.ID_RegistroEntidad = idRegEntidad == -1 ? int.Parse(cBoxReglas.SelectedItem.Value) :
                    idRegEntidad;
                NuevoValor.Valor = ValorDefault;
                NuevoValor.ValorAlertar = ValorAlerta;
                NuevoValor.ValorRechazar = ValorRechaza;
                NuevoValor.ValorBloquear = ValorBloquea;

                LNEfectivale.ModificaValorParametroRegla(NuevoValor, this.Usuario);

                X.Msg.Notify("Actualización de Parámetros", "Modificación <br />  <br /> <b>  A U T O R I Z A D A  </b> <br />  <br /> ").Show();

                if (cBoxNivelConfig.SelectedItem.Text.ToUpper() == "TARJETA")
                {
                    if (this.hdnEstatus.Text == "BLOQUEADA")
                    {
                        this.btnEstatusTarjeta.Show();
                    }
                    if (DAOEfectivaleOnline.ExistenIncidenciasTarjeta(this.hdnClaveEntidad.Text, this.Usuario))
                    {
                        this.btnCerrarCaso.Show();
                    }

                    this.ToolbarTarjeta.Hidden = this.btnEstatusTarjeta.Visible ? this.btnCerrarCaso.Visible ? false : true : true;
                }

                LlenaPanelParametros(NuevoValor.ID_RegistroEntidad);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualización de Parámetros", "Error al Actualizar los Valores de la Regla").Show();
            }
        }


        /// <summary>
        /// Realiza las validaciones para desbloquear la tarjeta
        /// </summary>
        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void DesbloqueaTarjeta()
        {
            try
            {
                LNEfectivale.DesbloqueaTarjeta(this.hdnClaveEntidad.Text, this.Usuario);

                X.Msg.Notify("Desbloquear Tarjeta", "Desbloqueo de Tarjeta  <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();

                this.btnEstatusTarjeta.Hide();

                this.ToolbarTarjeta.Hidden = this.btnCerrarCaso.Visible ? false : true;
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Desbloquear Tarjeta", "Ocurrió un Error con el Desbloqueo de la Tarjeta").Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Desbloquear Tarjeta", ex.Message).Show();
            }
        }

        /// <summary>
        /// Realiza las validaciones para el cierre de caso de una tarjeta
        /// </summary>
        [DirectMethod(Namespace = "ValidacionesBatch")]
        public void CierraCaso()
        {
            try
            {
                LNEfectivale.CierraCaso("NORM", -1, this.hdnClaveEntidad.Text, this.Usuario);

                X.Msg.Notify("Cerrar Caso", "Cierre de Caso <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();

                this.btnCerrarCaso.Hide();

                this.ToolbarTarjeta.Hidden = this.btnEstatusTarjeta.Visible ? true : false;
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cerrar Caso", "Ocurrió un Error con el Cierre del Caso").Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cerrar Caso", ex.Message).Show();
            }
        }
    }
}