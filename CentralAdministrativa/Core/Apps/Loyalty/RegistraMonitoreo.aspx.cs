using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.BaseDatos;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;

namespace CentroContacto
{
    public partial class RegistraMonitoreo : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Estado de Movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    EstableceDatosMonitoreo();

                    Response.AppendHeader("Refresh", Configuracion.Get(
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "TiempoRefreshMonitoreo").Valor);
                }
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Solicita a base de datos los datos de monitoreo para registro y 
        /// los establece en el grid de propiedades
        /// </summary>
        protected void EstableceDatosMonitoreo()
        {
            try
            {
                this.StoreDatosMonitoreo.RemoveAll();
                
                this.StoreDatosMonitoreo.DataSource = DAOMonitoreo.ListaDatosParaRegistro(
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                this.StoreDatosMonitoreo.DataBind();
            }

            catch (CAppException err)
            {
                throw new Exception(err.Message);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Refresh
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                EstableceDatosMonitoreo();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Consulta de datos", ex.Message);
            }
        }

        /// <summary>
        /// Actualiza el valor del dato de monitoreo con el ID indicado
        /// </summary>
        /// <param name="IdDatoMonitoreo">Identificador del dato de monitoreo</param>
        /// <param name="Valor">Valor del dato</param>
        [DirectMethod(Namespace = "RegistroMonitoreo")]
        public void ActualizaDato(int IdDatoMonitoreo, string Valor)
        {
            try
            {
                DAOMonitoreo.ActualizaRegistroBitacora(IdDatoMonitoreo, Valor, this.Usuario);

                X.Msg.Notify("", "Actualización de dato <br />  <br /> <b> E X I T O S A  </b> <br />  <br /> ").Show();

                EstableceDatosMonitoreo();
            }

            catch (CAppException err)
            {
                Loguear.Error(err, "");
                X.Msg.Alert("Actualización de Datos", err.Message);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                X.Msg.Alert("Actualización de Datos", ex.Message);
            }
        }
    }
}