using DALCentralAplicaciones;
using DALEventos.BaseDatos;
using DALEventos.LogicaNegocio;
using DALEventos.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Cortador
{
    public partial class AsignarCadenas : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    StorePeriodos.DataSource = DAOEvento.ListaPeriodos(
                               this.Usuario,
                               Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    StorePeriodos.DataBind();
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }


        /// <summary>
        /// Controla el evento Click en el botón de búsqueda de evento
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void btnBuscarCadena_Click(object sender, DirectEventArgs e)
        {
            try
            {
                StoreConsultaColectivas.RemoveAll();

                DataSet dsCadenas = DAOEvento.ConsultaCadenasComerciales(
                        txtClaveColectiva.Text, txtNombreCadena.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (dsCadenas.Tables[0].Rows.Count < 1)
                {
                    X.Msg.Alert("Búsqueda de Cadena Comercial", "No existen coincidencias con la búsqueda solicitada.").Show();
                    return;
                }
                else
                {
                    StoreConsultaColectivas.DataSource = dsCadenas;
                    StoreConsultaColectivas.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consulta de Evento", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Consulta de Evento", "Ocurrió un Error al Buscar los Eventos").Show();
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de cadenas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectCadena_Event(object sender, DirectEventArgs e)
        {
            try
            {
                string json = e.ExtraParams["Values"];

                IDictionary<string, string>[] cadenaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (cadenaSeleccionada == null || cadenaSeleccionada.Length < 1)
                {
                    return;
                }

                int idCadena = 0;
                string cadena = "";

                foreach (KeyValuePair<string, string> configCorte in cadenaSeleccionada[0])
                {
                    switch (configCorte.Key)
                    {
                        case "ID_Colectiva": idCadena = int.Parse(configCorte.Value); break;
                        case "NombreCadena": cadena = configCorte.Value; break;
                        default:
                            break;
                    }
                }

                LlenaGridCortes(idCadena, cadena);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cadena Comercial", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Cadena Comercial", "Ocurrió un Error al Seleccionar la Cadena").Show();
            }
        }

        /// <summary>
        /// Controla los comandos del grid de consulta de eventos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void selectComand_Event(object sender, DirectEventArgs e)
        {
            int idCadena = 0;
            string nombreCadena = "";

            string json = String.Format("[{0}]", e.ExtraParams["Values"]);

            IDictionary<string, string>[] cadenaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

            if (cadenaSeleccionada == null || cadenaSeleccionada.Length < 1)
            {
                return;
            }

            String command = (String)e.ExtraParams["Comando"];

            switch (command)
            {
                case "Select":
                    foreach (KeyValuePair<string, string> cadena in cadenaSeleccionada[0])
                    {
                        switch (cadena.Key)
                        {
                            case "ID_Colectiva": idCadena = int.Parse(cadena.Value); break;
                            case "NombreCadena": nombreCadena = cadena.Value; break;
                            default:
                                break;
                        }
                    }
                    break;

                default:
                    break;
            }


            LlenaGridCortes(idCadena, nombreCadena);
        }

        /// <summary>
        /// Consulta a base de datos los cortes de la cadena indicada y llena el grid correspondiente
        /// </summary>
        /// <param name="idCadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="nombreCadena">Nombre de la cadena comercial</param>
        protected void LlenaGridCortes(int idCadenaComercial, string nombreCadena)
        {
            try
            {
                StoreCortes.RemoveAll();

                StoreCortes.DataSource = DAOEvento.ConsultaCortesCadena(
                       idCadenaComercial, this.Usuario,
                       Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                StoreCortes.DataBind();

                FormPanelCortes.Title += " de " + nombreCadena;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cortes", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Cortes", "Ocurrió un Error al Consultar los Cortes de la Cadena").Show();
            }
        }

        /// <summary>
        /// Recibe la solicitud de inserción o actualización de un corte del frontend y la
        /// canaliza a base de datos
        /// </summary>
        /// <param name="IdAsignacion">Identificador del corte asignado</param>
        /// <param name="IdCadena">Identificador de la cadena comercial</param>
        /// <param name="Cadena">Nombre de la cadena comercial</param>
        /// <param name="IdCfgCorte">Identificador de la configuración de corte</param>
        /// <param name="IdPeriodo">Identificador del periodo</param>
        [DirectMethod(Namespace = "Cortador")]
        public void ActualizaPeriodo(int IdAsignacion, int IdCadena, string Cadena, int IdCfgCorte, int IdPeriodo)
        {
            try
            {
                LNEvento.CreaModificaPeriodoCorteCadena(IdAsignacion, IdCadena, IdCfgCorte, IdPeriodo, this.Usuario);

                X.Msg.Notify("Corte", "Periodo Actualizado Exitosamente").Show();

                LlenaGridCortes(IdCadena, Cadena);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Corte", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Corte", "No es Posible Actualizar el Periodo.").Show();
            }
        }

        /// <summary>
        /// Controla la ejecución de comandos del grid de eventos por agrupar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void EjecutaComando(object sender, DirectEventArgs e)
        {
            int idAsignacion = 0;
            int idCadena = 0;
            string cadena = "";
            int idCfgCorte = 0;
            int idPeriodo = 0;
            bool activo = false;

            try
            {
                string json = String.Format("[{0}]", e.ExtraParams["Values"]);

                IDictionary<string, string>[] corteSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (corteSeleccionado == null || corteSeleccionado.Length < 1)
                {
                    return;
                }

                String command = (String)e.ExtraParams["Comando"];

                switch (command)
                {
                    case "Link":
                    case "Unlink":
                        foreach (KeyValuePair<string, string> corte in corteSeleccionado[0])
                        {
                            switch (corte.Key)
                            {
                                case "ID_Asignacion": idAsignacion = int.Parse(corte.Value); break;
                                case "ID_CadenaComercial": idCadena = int.Parse(corte.Value); break;
                                case "Cadena": cadena = corte.Value; break;
                                case "ID_ConfiguracionCorte": idCfgCorte = int.Parse(corte.Value); break;
                                case "ID_Periodo": idPeriodo = int.Parse(corte.Value); break;
                                case "Activo": activo = bool.Parse(corte.Value); break;
                                default:
                                    break;
                            }
                        }

                        LNEvento.CreaModificaCorteCadena(idAsignacion, idCadena, idCfgCorte, idPeriodo, this.Usuario);
                        X.Msg.Notify("Corte", "Corte " + (activo ? "Desasignado" : "Asignado") + " Exitosamente").Show();

                        LlenaGridCortes(idCadena, cadena);
                        break;

                    default:
                        break;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Corte", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Corte", "No es Posible Actualizar el Periodo.").Show();
            }
        }
    }
}