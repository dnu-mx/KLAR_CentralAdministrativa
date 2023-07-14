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
    public partial class DepositoRecolector : DALCentralAplicaciones.PaginaBaseCAPP
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
                    X.Msg.Alert("Depósito de Recolector", "Captura el importe del depósito").Show();
                    return;
                }

                if (!String.IsNullOrEmpty(this.txtReferencia.Text))
                {
                    Int64 cel;

                    if (!Int64.TryParse(this.txtReferencia.Text, out cel))
                    {
                        X.Msg.Alert("Depósito de Recolector", "La referencia debe ser numérica").Show();
                        return;
                    }
                }

                EventoDepositoRecolector deposito = new EventoDepositoRecolector();

                DataSet dsEvento = DAOEvento.ConsultaEventoDepositoRecolectorDiconsa
                    (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                deposito.IdTipoColectivaOrigen = Convert.ToInt32(dsEvento.Tables[0].Rows[0]["IdTipoColectivaOrigen"].ToString());
                deposito.IdColectivaOrigen = Convert.ToInt32(cmbRecolector.SelectedItem.Value);
                deposito.ClaveColectiva = dsEvento.Tables[0].Rows[0]["ClaveColectiva"].ToString().Trim();
                deposito.IdEvento = Convert.ToInt32(dsEvento.Tables[0].Rows[0]["ID_Evento"].ToString());
                deposito.ClaveEvento = dsEvento.Tables[0].Rows[0]["ClaveEvento"].ToString();
                deposito.Concepto = dsEvento.Tables[0].Rows[0]["Descripcion"].ToString();
                deposito.Importe = this.nfImporte.Text;
                deposito.Referencia = Convert.ToInt64(this.txtReferencia.Text);
                deposito.Observaciones = this.txtObservaciones.Text;

                LogHeader LH_DepRecDiconsa = new LogHeader();
                LNEvento.RegistraEvManual_DepRecolectorDiconsa(deposito, this.Usuario, LH_DepRecDiconsa);

                X.Msg.Notify("", "El Depósito del Recolector se Registró Exitosamente").Show();

                this.FormPanel1.Reset();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Depósito de Recolector", err.Mensaje()).Show();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Depósito de Recolector", "Ocurrió un Error al Registrar el Depósito del Recolector").Show();
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