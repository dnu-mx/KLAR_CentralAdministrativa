using DALAutorizador.Entidades;
using DALCortador.Entidades;
using DALEventos.BaseDatos;
using DALEventos.LogicaNegocio;
using DALEventos.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Cortador
{
    public partial class TraspasosCuentasMismoTipo : DALCentralAplicaciones.PaginaBaseCAPP
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
                    LlenaComboTraspasos();
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Solicita el catálogo de traspasos a base de datos y lo establece en 
        /// el combo correspondiente
        /// </summary>
        protected void LlenaComboTraspasos()
        {
            try
            {
                StoreTraspasos.DataSource = DAOEvento.ListaEventosTraspaso(
                            this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreTraspasos.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Traspasos", err.Mensaje()).Show();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Traspasos", "Ocurrió un Error al Consultar los Eventos").Show();
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Solicita los catálogos de las colectivas de tipo origen y destino y los
        /// establece en los combos correspondientes
        /// </summary>
        /// <param name="IdTipoCuenta">Identificador del tipo de cuenta</param>
        [DirectMethod(Namespace = "Traspasos")]
        public void LlenaColectivas(int IdTipoCuenta)
        {
            try
            {
                DataSet dsColectivas = DAOEvento.ListaColectivasTraspasos(IdTipoCuenta,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (dsColectivas.Tables[0].Rows.Count < 1)
                {
                    X.Msg.Alert("Origen/Destino", "No existen elementos origen y/o destino para este tipo de traspaso.").Show();
                    return;
                }
                else
                {
                    List<ColectivaComboPredictivo> colectivaList = new List<ColectivaComboPredictivo>();

                    foreach (DataRow colectiva in dsColectivas.Tables[0].Rows)
                    {
                        var colectivaCombo = new ColectivaComboPredictivo()
                        {
                            ID_Colectiva = Convert.ToInt64(colectiva["ID_Colectiva"].ToString()),
                            ClaveColectiva = colectiva["ClaveColectiva"].ToString(),
                            NombreORazonSocial = colectiva["NombreORazonSocial"].ToString()
                        };
                        colectivaList.Add(colectivaCombo);
                    }

                    StoreColectivas.DataSource = colectivaList;
                    StoreColectivas.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Traspasos", err.Mensaje()).Show();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Traspasos", "Ocurrió un Error al Consultar Origen y/o Destino").Show();
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Controla el evento click al botón Limpiar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiaFormuario();
        }

        /// <summary>
        /// Restablece los controles del formulario a su estatus de carga inicial
        /// </summary>
        protected void LimpiaFormuario()
        {
            cBoxEvento.Reset();

            cBoxOrigen.Reset();
            cBoxDestino.Reset();
            StoreColectivas.RemoveAll();
            
            txtImporte.Reset();
            txtReferencia.Reset();
            txtObservaciones.Reset();
        }

        /// <summary>
        /// Controla el evento Click al botón de Traspasar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnTraspasar_Click(object sender, DirectEventArgs e)
        {
            try
            {
                char[] charsToTrim = { '$', ' ' };
                EventoManual elEvento = new EventoManual();

                elEvento.IdEvento = int.Parse(cBoxEvento.SelectedItem.Value);
                elEvento.Concepto = cBoxEvento.SelectedItem.Text;

                elEvento.IdColectivaOrigen = Convert.ToInt64(cBoxOrigen.SelectedItem.Value);
                elEvento.IdTipoColectivaOrigen = Convert.ToInt32(hdnTipoColOrigen.Value);
                elEvento.IdColectivaDestino = Convert.ToInt64(cBoxDestino.SelectedItem.Value);
                elEvento.IdTipoColectivaDestino = Convert.ToInt32(hdnTipoColDestino.Value);

                elEvento.Importe = txtImporte.Text.Trim(charsToTrim);
                elEvento.Referencia = String.IsNullOrEmpty(txtReferencia.Text) ? 0 : Convert.ToInt64(txtReferencia.Text);
                elEvento.Observaciones = txtObservaciones.Text;

                LogHeader LH_TrasCtasMT = new LogHeader();
                LNEvento.RegistraEventoTraspasoCuentasMismoTipo(elEvento, this.Usuario, LH_TrasCtasMT);

                LimpiaFormuario();

                X.Msg.Notify("Traspasos", "El Traspasos se Registró <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Traspasos", err.Mensaje()).Show();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Traspasos", ex.Message).Show();
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }
    }
}