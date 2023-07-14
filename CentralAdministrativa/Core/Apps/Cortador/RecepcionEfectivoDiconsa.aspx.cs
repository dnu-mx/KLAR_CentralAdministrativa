using DALCortador.Entidades;
using DALEventos.BaseDatos;
using DALEventos.LogicaNegocio;
using DALEventos.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Data;

namespace Cortador
{
    public partial class RecepcionEfectivoDiconsa : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //Se llena el combo de recolectores
                    StoreRecolector.DataSource = DAOEvento.ListaRecolectoresDiconsa(
                                this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    StoreRecolector.DataBind();
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }


        /// <summary>
        /// Controla el evento de selección de algún ítem del combo de Recolectores
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        public void LlenaTxtImporte(object sender, EventArgs e)
        {
            try
            {
                string Saldo = DAOEvento.ConsultaSaldoRecolectorDiconsa
                                    (int.Parse(cmbRecolector.SelectedItem.Value), this.Usuario);

                this.nfImporte.Text = String.IsNullOrEmpty(Saldo) ? "0" : Saldo;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Regsitrar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnRegistrar_Click(object sender, DirectEventArgs e)
        {
            try
            {
                if (Convert.ToDecimal(nfImporte.Text) == 0)
                {
                    X.Msg.Alert("Recepción de Efectivo", "Captura el importe recibido").Show();
                    return;
                }

                if (!String.IsNullOrEmpty(this.txtReferencia.Text))
                {
                    Int64 cel;

                    if (!Int64.TryParse(this.txtReferencia.Text, out cel))
                    {
                        X.Msg.Alert("Recepción de Efectivo", "La referencia debe ser numérica").Show();
                        return;
                    }
                }

                EfectivoDiconsa efectivoDic = new EfectivoDiconsa();

                DataSet dsEvento = DAOEvento.ConsultaEventoRecepcionEfectivoDiconsa
                    (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));


                efectivoDic.IdEvento = Convert.ToInt32(dsEvento.Tables[0].Rows[0]["IdEvento"].ToString());
                efectivoDic.Concepto = dsEvento.Tables[0].Rows[0]["Concepto"].ToString();
                efectivoDic.IdColectivaOrigen = Convert.ToInt64(cmbRecolector.SelectedItem.Value);
                efectivoDic.IdTipoColectivaOrigen = Convert.ToInt32(dsEvento.Tables[0].Rows[0]["IdTipoColectivaOrigen"].ToString());
                efectivoDic.IdColectivaDestino = Convert.ToInt32(dsEvento.Tables[0].Rows[0]["IdColectivaDestino"].ToString());
                efectivoDic.IdTipoColectivaDestino = Convert.ToInt32(dsEvento.Tables[0].Rows[0]["IdTipoColectivaDestivo"].ToString());
                efectivoDic.Importe = this.nfImporte.Text;
                efectivoDic.Observaciones = this.txtObservaciones.Text;
                efectivoDic.Referencia = this.txtReferencia.Text == "" ? 0 : Convert.ToInt64(this.txtReferencia.Text);

                LogHeader LH_RecEfDiconsa = new LogHeader();
                LNEvento.RegistraEvManual_RecepEfectDiconsa(efectivoDic, LH_RecEfDiconsa);

                X.Msg.Notify("", "La Recepción de Efectivo se Registró Exitosamente").Show();

                this.FormPanel1.Reset();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Recepción de Efectivo", err.Mensaje()).Show();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Recepción de Efectivo", "Ocurrió un Error al Registrar la Recepción de Efectivo").Show();
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar Registro
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnNuevoRegistro_Click(object sender, DirectEventArgs e)
        {
            this.FormPanel1.Reset();
        }
    }
}