using DALAdministracion.BaseDatos;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Administracion
{
    public partial class ConfiguradorBines : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    LogHeader logTEMP = new LogHeader();
                    DataSet dsTiposColectiva = DAOColectiva.ListaTiposColectivaSubemisor(
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        logTEMP);

                    StoreTipoColectiva.DataSource = dsTiposColectiva;
                    StoreTipoColectiva.DataBind();

                    StoreTipoIntegracion.DataSource = DAOProducto.ListaTiposIntegracion(
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        logTEMP);
                    StoreTipoIntegracion.DataBind();


                    if (!X.IsAjaxRequest)
                    {
                    }
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar, restableciendo los controles
        /// a su estado de carga inicial
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            FormPanelBusqueda.Reset();

            LimpiaBusquedaPrevia();
        }

        /// <summary>
        /// Restablece a su estado de carga inicial a los controles relacionados con
        /// el resultado de una búsqueda previa
        /// </summary>
        protected void LimpiaBusquedaPrevia()
        {
            StoreColectivas.RemoveAll();

            RestableceGridsGMABines();
        }

        /// <summary>
        /// Restablece a su estado de carga iniciallos grids de Grupo de Tarjetas y Bines
        /// </summary>
        protected void RestableceGridsGMABines()
        {
            PanelGpoMA.Title = "Grupo de Tarjetas";
            StoreGrupoMA.RemoveAll();

            GridBines.Title = "Bines";
            StoreBines.RemoveAll();

            btnNuevoBin.Disabled = true;
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar, invocando la consulta de colectivas
        /// en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaBusquedaPrevia();

            LlenaGridColectivas();
        }

        /// <summary>
        /// Llena el grid de grupos de medios de acceso con la información de base de datos
        /// </summary>
        protected void LlenaGridColectivas()
        {
            try
            {
                LogHeader logTEMP = new LogHeader();
                DataSet dsColectivas = DAOColectiva.ListaColectivasPorTipo(
                    int.Parse(this.cBoxTipoColec.SelectedItem.Value), this.txtColectiva.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    logTEMP);

                if (dsColectivas.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Colectivas", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreColectivas.DataSource = dsColectivas;
                    StoreColectivas.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Colectivas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Colectivas", "Ocurrió un Error al Buscar la Colectiva").Show();
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowColectivas_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdColectiva = 0;
                string Colectiva = "";
                RestableceGridsGMABines();

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] colectiva = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in colectiva[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Colectiva": IdColectiva = int.Parse(column.Value); break;
                        case "NombreORazonSocial": Colectiva = column.Value; break;
                        default: break;
                    }
                }

                hdnIdColectiva.Value = IdColectiva;
                hdnColectiva.Value = Colectiva;
                LlenaGridGruposMA(IdColectiva, Colectiva);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Colectivas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                X.Msg.Alert("Colectivas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el grid de grupos de medios de acceso con la información de base de datos
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva por buscar</param>
        /// <param name="Colectiva">Nombre o razón social de la colectiva</param>
        protected void LlenaGridGruposMA(int IdColectiva, string Colectiva)
        {
            try
            {
                StoreGrupoMA.RemoveAll();

                DataTable dtClientes = DAOGruposMA.ListaGruposMABines(IdColectiva, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (dtClientes.Rows.Count < 1)
                {
                    X.Msg.Alert("Grupos de Tarjetas", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    PanelGpoMA.Title += " - " + Colectiva;

                    StoreGrupoMA.DataSource = dtClientes;
                    StoreGrupoMA.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Grupo de Tarjetas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Grupo de Tarjetas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de grupos de medios de acceso
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void ComandoGMA(object sender, DirectEventArgs e)
        {
            try
            {
                String comando = e.ExtraParams["Comando"];
                int IdGrupoMA = int.Parse(e.ExtraParams["ID_GrupoMA"].ToString());
                hdnIDGrupoMA.Value = IdGrupoMA;
                String grupoMA = e.ExtraParams["Descripcion"];

                switch (comando)
                {
                    case "BinesGMA":
                        GridBines.Title += " - " + grupoMA;
                        LlenaGridBines(IdGrupoMA);
                        break;

                    default:
                        break;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Grupo de Tarjetas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                X.Msg.Alert("Grupo de Tarjetas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el grid de bines con los datos obtenidos de la consulta a base de datos
        /// </summary>
        /// <param name="IdGrupoMA">Identificador del grupo de medios de acceso seleccionado</param>
        protected void LlenaGridBines(int IdGrupoMA)
        {
            try
            {
                StoreBines.RemoveAll();
                btnNuevoBin.Disabled = false;

                DataTable dtBines = DAOTarjeta.ListaBinesGrupoMA(IdGrupoMA,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (dtBines.Rows.Count < 1)
                {
                    X.Msg.Alert("Bines", "El Grupo de Tarjetas no tiene Bines asociados.").Show();
                }

                StoreBines.DataSource = dtBines;
                StoreBines.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Bines", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Bines", ex.Message).Show();
            }
        }

        /// <summary>
        /// Recibe la solicitud de actualización del tipo de integración del grupo de medios de acceso
        /// y la canaliza a base de datos
        /// </summary>
        /// <param name="IdGrupoMA">Identificador del corte asignado</param>
        [DirectMethod(Namespace = "Bines")]
        public void ActualizaTipoSaldo(int IdGMA)
        {
            try
            {
                LNProductos.ModificaTipoIntegracionGrupoMA(IdGMA, int.Parse(cBoxTipoIntegracion.SelectedItem.Value), this.Usuario);

                X.Msg.Notify("Tipo de Saldo", "Actualizado <br /><br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridGruposMA(Convert.ToInt32(hdnIdColectiva.Value), hdnColectiva.Value.ToString());
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tipo de Saldo", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Tipo de Saldo", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de cuentas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            try
            {
                String comando = (String)e.ExtraParams["Comando"];
                int ID_BINGrupoMA = Convert.ToInt32(e.ExtraParams["ID_BINGrupoMA"]);

                int estatus = comando == "Lock" ? 0 : 1;
                string msj = comando == "Lock" ? "Desactivado" : "Activado";

                if (estatus == 1)
                {
                    try
                    {
                        LNTarjetas.AsignaBinGrupoMA(ID_BINGrupoMA, Convert.ToInt32(hdnIDGrupoMA.Value), this.Usuario);
                    }
                    catch (CAppException caEx)
                    {
                        throw caEx;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                LNTarjetas.ModificaEstatusBinGrupoMA(ID_BINGrupoMA, estatus, this.Usuario);

                X.Msg.Notify("", "BIN " + msj + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridBines(Convert.ToInt32(hdnIDGrupoMA.Value));
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Acción", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Acción", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Crear Bin, invocando a la creación del nuevo
        /// BIN en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnCrearNuevoBin_Click(object sender, DirectEventArgs e)
        {
            try
            {
                LNTarjetas.CreaNuevoBinGrupoMA(Convert.ToInt32(hdnIDGrupoMA.Value), txtClaveBin.Text,
                    txtDescripcionBin.Text, this.Usuario);

                WdwNuevoBin.Hide();
                X.Msg.Notify("Nuevo Bin", "BIN creado <br /><br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridBines(Convert.ToInt32(hdnIDGrupoMA.Value));
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Bin", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Notify("Nuevo Bin", ex.Message).Show();
            }
        }
    }
}