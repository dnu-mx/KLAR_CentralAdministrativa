using DALCentralAplicaciones;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace TpvWeb
{
    public partial class CVDP : PaginaBaseCAPP
    {
        private LogHeader LH_CVDP = new LogHeader();


        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_CVDP.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_CVDP.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_CVDP.User = this.Usuario.ClaveUsuario;
            LH_CVDP.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_CVDP);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA CVDP Page_Load()");

                if (!IsPostBack)
                {
                    ;
                }

                log.Info("TERMINA CVDP Page_Load()");
            }

            catch (Exception err)
            {
                log.ErrorException(err);
                errRedirect = "../ErrorInicializarPagina.aspx";
            }

            finally
            {
                if (!string.IsNullOrEmpty(errRedirect))
                {
                    Response.Redirect(errRedirect, false);

                    Response.Flush();
                    Response.Close();
                }
            }
        }

        protected void LimpiaControles()
        {
            this.StoreGrupoMA.RemoveAll();

            this.StoreValoresParametros.RemoveAll();
            this.GridPanelParametros.Disabled = true;
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar, restableciendo
        /// todos los controles, páneles y grids a su estado de carga inicial
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.txtDescripcion.Reset();
            LimpiaControles();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaControles();
            LlenaGridResultados();
        }
    
        protected void LlenaGridResultados()
        {
            LogPCI unLog = new LogPCI(LH_CVDP);

            try
            {
                unLog.Info("INICIA ObtieneGruposMa()");
                DataSet ds = DAOCVDP.ObtieneGruposMa(this.txtDescripcion.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_CVDP);
                unLog.Info("TERMINA ObtieneGruposMa()");

                if (ds.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Grupo de Tarjetas", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    this.StoreGrupoMA.DataSource = ds;
                    this.StoreGrupoMA.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Grupo de Tarjetas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Grupo de Tarjetas", "Ocurrió un error al realizar la bÚsqueda.").Show();
            }
        }

        protected void LlenaGridParametros()
        {
            LogPCI unLog = new LogPCI(LH_CVDP);

            try
            {
                unLog.Info("INICIA ObtieneValoresParametros()");
                DataSet ds = DAOCVDP.ObtieneValoresParametros(Convert.ToInt32(this.hdnIdGrupoMA.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_CVDP);
                unLog.Info("TERMINA ObtieneValoresParametros()");

                if (ds.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Parámetros", "El Grupo de Tarjetas seleccionado no cuenta con Parámetros configurados.").Show();
                }
                else
                {
                    this.StoreValoresParametros.DataSource = ds;
                    this.StoreValoresParametros.DataBind();
                    this.GridPanelParametros.Disabled = false;
                    this.GridPanelParametros.Title = "Parámetros de " + this.hdnDescripcion.Text;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Parámetros", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al realizar la bÚsqueda.").Show();
            }
        }

        protected void selectRowResultados_Event(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_CVDP);
            unLog.Info("selectRowResultados_Event()");

            try
            {
                int IdGrupoMA = 0;
                string Descripcion = "";
                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] grupoMA = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in grupoMA[0])
                {
                    switch (column.Key)
                    {
                        case "ID_GrupoMA": IdGrupoMA = int.Parse(column.Value); break;
                        case "Descripcion": Descripcion = column.Value; break;
                        default:
                            break;
                    }
                }

                this.hdnIdGrupoMA.Value = IdGrupoMA;
                this.hdnDescripcion.Text = Descripcion.ToString();

                LlenaGridParametros();
             
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Parámetros", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {                
                unLog.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al obtener los parámetros.").Show();
            }
        }

        /// <summary>
        /// Restablece los controles de la ventana de edición de parámetros
        /// </summary>
        protected void LimpiaVentanaParams()
        {
            this.FormPanelValorParamTxt.Reset();
            this.txtParametro.Reset();

            this.txtValorParFloat.Reset();
            this.txtValorParFloat.Hidden = true;

            this.txtValorParInt.Reset();
            this.txtValorParInt.Hidden = true;

            this.txtValorParString.Reset();
            this.txtValorParString.Hidden = true;

            this.cBoxValorPar.Reset();
            this.cBoxValorPar.Hidden = true;
        }

        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_CVDP);
            pCI.Info("EjecutarComandoParametros()");

            try
            {
                string param = string.Empty, nombre = string.Empty;
                string valor = string.Empty, tipoDato = string.Empty;
                string json = String.Format("[{0}]", e.ExtraParams["Values"]);

                IDictionary<string, string>[] parametroSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (parametroSeleccionado == null || parametroSeleccionado.Length < 1)
                {
                    return;
                }

                foreach (KeyValuePair<string, string> column in parametroSeleccionado[0])
                {
                    switch (column.Key)
                    {
                        case "ID_ValorReferido": this.hdnIdValorReferido.Value = column.Value; break;
                        case "Nombre": nombre = column.Value; break;
                        case "Descripcion": param = column.Value; break;
                        case "Valor": valor = column.Value; break;
                        case "TipoDato": tipoDato = column.Value; break;
                        default:
                            break;
                    }
                }

                String comando = e.ExtraParams["Comando"];

                switch (comando)
                {
                    case "Edit":
                        LimpiaVentanaParams();

                        switch (tipoDato.ToUpper())
                        {
                            case "BOOL":
                            case "BOOLEAN":
                                this.cBoxValorPar.Value = valor;
                                this.cBoxValorPar.Hidden = false;
                                break;

                            case "FLOAT":
                                this.txtValorParFloat.Value = valor;
                                this.txtValorParFloat.Hidden = false;
                                break;

                            case "INT":
                                this.txtValorParInt.Value = valor;
                                this.txtValorParInt.Hidden = false;
                                break;

                            case "STRING":
                                this.txtValorParString.Value = valor;
                                this.txtValorParString.Hidden = false;
                                break;
                        }

                        this.txtParametro.Text = param;
                        this.WdwValorParametro.Title += " - " + nombre;
                        this.WdwValorParametro.Show();
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGuardarValorParametro_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_CVDP);

            try
            {
                unLog.Info("INICIA ModificaValorReferidoPertenencia()");
                LNEscrituraGeneral.ModificaValorReferidoPertenencia(
                    Convert.ToInt32(this.hdnIdGrupoMA.Value),
                    Convert.ToInt32(this.hdnIdValorReferido.Value), 
                    String.IsNullOrEmpty(this.txtValorParFloat.Text) ?
                    String.IsNullOrEmpty(this.txtValorParInt.Text) ?
                    String.IsNullOrEmpty(this.txtValorParString.Text) ?
                    this.cBoxValorPar.SelectedItem.Value : this.txtValorParString.Text :
                    this.txtValorParInt.Text : this.txtValorParFloat.Text, LH_CVDP);
                unLog.Info("TERMINA ModificaValorReferidoPertenencia()");


                this.WdwValorParametro.Hide();

                X.Msg.Notify("Parámetro", "Modificación de Parámetro <br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();

                LlenaGridParametros();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Parámetro", "Ocurrió un error al modificar el Parámetro").Show();
            }
        }
    }
}