using CentroContacto.LogicaNegocio;
using ClosedXML.Excel;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.Entidades;
using DALCentroContacto.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;

namespace CentroContacto
{
    public partial class ConsultaClientes_SmartTicketMC : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        protected enum AccionBitacora
        {
            CambioContrasena = 43,
            CambioEstatus = 42,
            Bloqueo = 44
        };

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Consulta de Clientes
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    StoreEstatusConfirma.DataSource = new object[]
                    {
                        new object[]{"1" , "Confirmado"},
                        new object[]{"2" , "Pendiente de Confirmar"},
                        new object[]{"3" , "Pendiente de Cambio de Contraseña"}
                    };
                    StoreEstatusConfirma.DataBind();

                    //Prestablecemos las fechas de consulta de los Pedidos
                    dfFechaInicialPed.MaxDate = DateTime.Today;
                    dfFechaInicialPed.SetValue(DateTime.Today.AddDays(-30));

                    dfFechaFinalPed.MaxDate = DateTime.Today;
                    dfFechaFinalPed.SetValue(DateTime.Today);

                    FormPanelDatos.Show();
                }
            }

            catch (Exception err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Llena el grid de resultado de búsqueda de clientes
        /// </summary>
        protected void LlenarGridResultados()
        {
            try
            {
                ClienteColectiva elCliente = new ClienteColectiva();

                elCliente.Nombre = this.txtNombre.Text;
                elCliente.ApellidoPaterno = this.txtApPaterno.Text;
                elCliente.ApellidoMaterno = this.txtApMaterno.Text;
                elCliente.FechaNacimiento = Convert.ToDateTime(this.dfFechaNac.Value);
                elCliente.Email = this.txtCorreo.Text;
                elCliente.Telefono = this.nfTelefono.Text;

                DataSet dsResultados = LNClientesSmartTicketMC.ConsultaClientes(elCliente, this.Usuario);

                limpiaBusquedaPrevia(false);

                int numRecords = dsResultados.Tables[0].Rows.Count;

                if (numRecords == 100)
                {
                    X.Msg.Alert("Consulta de Clientes", "Demasiadas coincidencias, por favor afine su búsqueda").Show();
                }
                else if (numRecords == 0)
                {
                    X.Msg.Alert("Consulta de Clientes", "No existen coincidencias con la búsqueda solicitada").Show();
                }

                GridResultados.GetStore().DataSource = dsResultados;
                GridResultados.GetStore().DataBind();
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Clientes", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Clientes", "Ocurrió un Error al Ejecutar la Búsqueda con los Datos Proporcionados").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario de búsqueda, invocando la búsqueda
        /// de clientes a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenarGridResultados();
        }

        /// <summary>
        /// Controla el evento Refresh en el grid de resultados, invocando nuevamente
        /// la búsqueda de clientes a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento Refresh Data del Store Clientes</param>
        protected void StoreClientes_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            LlenarGridResultados();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de un cliente en
        /// el Grid de Resultados Clientes
        /// </summary>
        protected void limpiaSeleccionPrevia()
        {
            FormPanelDatos.Reset();
            StoreResultadosPed.RemoveAll();
            dfFechaInicialPed.SetValue(DateTime.Today.AddDays(-30));
            dfFechaFinalPed.SetValue(DateTime.Today);
            btnExcelPed.Disabled = true;

            FormPanelAjustesManuales.Reset();
            FormPanelDatos.Show();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la búsqueda previa de clientes dentro
        /// del Grid de Resultados Clientes
        /// </summary>
        /// <param name="esBtnLimpiar">Indica si el método de origen fue el clic al botón Limpiar</param>
        protected void limpiaBusquedaPrevia(bool esBtnLimpiar)
        {
            if (esBtnLimpiar)
            {
                FormPanelBusqueda.Reset();
            }

            StoreClientes.RemoveAll();
            FormPanelResultados.Reset();

            FormPanelDatos.Reset();

            StoreResultadosPed.RemoveAll();
            dfFechaInicialPed.SetValue(DateTime.Today.AddDays(-30));
            dfFechaFinalPed.SetValue(DateTime.Today);

            FormPanelAjustesManuales.Reset();
            FormPanelDatos.Show();
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario de búsqueda, limpiando los controles,
        /// páneles y grids asociados a alguna previa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiaBusquedaPrevia(true);
        }


        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowResultados_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdCliente = 0, IdColectiva = 0, IdTipoColectiva = 0;
                String eMailCliente = "", EstatusMA = "";

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] clienteSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in clienteSeleccionado[0])
                {
                    switch (column.Key)
                    {
                        case "IdCliente": IdCliente = int.Parse(column.Value); break;
                        case "IdColectiva": IdColectiva = int.Parse(column.Value); break;
                        case "IdTipoColectiva": IdTipoColectiva = int.Parse(column.Value); break;
                        case "Email": eMailCliente = column.Value; break;
                        case "EstatusMA": EstatusMA = column.Value; break;
                        default:
                            break;
                    }
                }

                limpiaSeleccionPrevia();
                LlenaFieldSetDatosCliente(IdCliente, IdColectiva, IdTipoColectiva, eMailCliente);

                PanelCentral.Collapsed = false;
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Resultados Clientes", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Resultados Clientes", "Ocurrió un Error al Seleccionar el Cliente").Show();
            }
        }

        /// <summary>
        /// Registra en bitácora la acción realizada en el formulario correspondiente
        /// </summary>
        /// <param name="Id_Cliente">Identificador del cliente</param>
        /// <param name="Formulario">AccionBitacora</param>
        protected void RegistraEnBitacora(int IdColectiva, int Formulario)
        {
            try
            {
                string Accion;

                switch (Formulario)
                {
                    case (ushort)AccionBitacora.CambioContrasena:
                        Accion = "Ajuste Manual SmartTicket MC - Cambio de Contraseña";
                        break;

                    case (ushort)AccionBitacora.CambioEstatus:
                        Accion = "Ajuste Manual SmartTicket MC - Confirmación de Correo";
                        break;

                    case (ushort)AccionBitacora.Bloqueo:
                        Accion = "Ajuste Manual SmartTicket MC - Bloqueo/Desbloqueo";
                        break;

                    default:    
                        Accion = "";
                        break;
                }

                DAOBitacoraActividades.InsertaActividadMoshiEnBitacora(IdColectiva, Accion, "", this.Usuario);
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Evento("NO se Insertó una Acción en la Bitácora de Actividades del Autorizador de Lealtad. " + ex.Message, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Consulta a BD los datos del cliente seleccionado y los llena en el FieldSet correspondiente,
        /// almacenando en controles ocultos los datos clave para las funcionalidades de las pestañas.
        /// </summary>
        /// <param name="idcliente">Identificador del cliente</param>
        /// <param name="idcolectiva">Identificador de la colectiva</param>
        /// <param name="idtipocolectiva">Identificador del tipo de colectiva del cliente</param>
        /// <param name="eMail">Correo electrónico</param>
        protected void LlenaFieldSetDatosCliente(int idcliente, int idcolectiva, int idtipocolectiva, String eMail)
        {
            try
            {
                FormPanelDatos.Reset();
                DataSet dsDatos = DAOConsultaClientesSmartTicketMC.ObtieneDatosCliente(idcliente, idcolectiva, this.Usuario);

                this.hdID_Cliente.Text = idcliente.ToString();
                this.hdID_Colectiva.Text = idcolectiva.ToString();
                this.hdID_TipoColectiva.Text = idtipocolectiva.ToString();
                this.hdeMail.Text = eMail;

                this.txtNombreCliente.Text = dsDatos.Tables[0].Rows[0]["Nombre"].ToString().Trim();
                this.txtApPaternoCliente.Text = dsDatos.Tables[0].Rows[0]["ApPaterno"].ToString().Trim();
                this.txtApMaternoCliente.Text = dsDatos.Tables[0].Rows[0]["ApMaterno"].ToString().Trim();

                this.txtEmailCliente.Text = dsDatos.Tables[0].Rows[0]["Email"].ToString().Trim();

                if (dsDatos.Tables[0].Rows[0]["FechaNacimiento"].Equals(""))
                {
                    this.dfFechaNacCliente.Clear();
                }
                else
                {
                    this.dfFechaNacCliente.Value = String.Format("{0:dd/MM/yyyy}",
                    Convert.ToDateTime(dsDatos.Tables[0].Rows[0]["FechaNacimiento"].ToString()));
                }


                if (dsDatos.Tables[0].Rows[0]["FechaAlta"].Equals(""))
                {
                    this.dfFechaAlta.Clear();
                }
                else
                {
                    this.dfFechaAlta.Value = String.Format("{0:dd/MM/yyyy}",
                    Convert.ToDateTime(dsDatos.Tables[0].Rows[0]["FechaAlta"].ToString()));
                }

                if (dsDatos.Tables[0].Rows[0]["FechaAltaLealtad"].Equals(""))
                {
                    this.dfFechaAltaLealtad.Clear();
                }
                else
                {
                    this.dfFechaAltaLealtad.Value = String.Format("{0:dd/MM/yyyy}",
                    Convert.ToDateTime(dsDatos.Tables[0].Rows[0]["FechaAltaLealtad"].ToString()));
                }

                    
                this.txtEmpresa.Text = dsDatos.Tables[0].Rows[0]["Empresa"].ToString().Trim();
                this.cboEstatusConfirma.SelectedItem.Value = dsDatos.Tables[0].Rows[0]["EstatusConfirma"].ToString().Trim();

                dsDatos = DAOConsultaClientesSmartTicketMC.ObtieneBloqueoCliente(eMail, this.Usuario);
                string bloqueo;

                if (dsDatos.Tables[0].Rows.Count == 0)
                {
                    bloqueo = "desbloqueado";
                    this.dfFechaBloqueo.Hidden = true;
                    this.dfFechaBloqueo.Clear();
                }
                else
                {
                    bloqueo = dsDatos.Tables[0].Rows[0]["Bloqueado"].Equals(true) ? "bloqueado" : "desbloqueado";

                    if (String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["FechaBloqueo"].ToString()))
                    {
                        this.dfFechaBloqueo.Clear();
                    }
                    else
                    {
                        this.dfFechaBloqueo.Value = String.Format("{0:dd/MM/yyyy}",
                        Convert.ToDateTime(dsDatos.Tables[0].Rows[0]["FechaBloqueo"].ToString()));
                    }

                    this.dfFechaBloqueo.Hidden = dsDatos.Tables[0].Rows[0]["Bloqueado"].Equals(true) ? false : true;
                }

                RadioGroupEstatus.SetValue(bloqueo);
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos del Cliente", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos del Cliente", "Ocurrió un Error al Consultar los Datos del Cliente").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario de pedidos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarPed_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.hdID_Cliente.Text))
                {
                    return;
                }

                StoreResultadosPed.RemoveAll();

                DataSet dsPedidos = new DataSet();

                ClienteColectiva elClienteSeleccionado = new ClienteColectiva();
                elClienteSeleccionado.ID_Cliente = Convert.ToInt32(this.hdID_Cliente.Text);
                elClienteSeleccionado.Email = this.hdeMail.Text;


                dsPedidos = LNClientesSmartTicketMC.ConsultaPedidos(
                elClienteSeleccionado,
                DateTime.Parse(dfFechaInicialPed.SelectedDate.ToString("yyyy-MM-dd HH:mm:ss")),
                DateTime.Parse(dfFechaFinalPed.SelectedDate.ToString("yyyy-MM-dd HH:mm:ss")),
                this.Usuario);

                if (dsPedidos.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Pedidos", "No Existen Pedidos del Cliente el Periodo Solicitado").Show();
                }
                else
                {
                    StoreResultadosPed.DataSource = dsPedidos;
                    StoreResultadosPed.DataBind();

                    btnExcelPed.Disabled = false;
                }
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Pedidos", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Pedidos", "Ocurrió un Error al Buscar los Pedidos").Show();
            }
        }

        /// <summary>
        /// Extrae los grids del backend para generarlos y exportarlos a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            string gridResultJson = e.ExtraParams["GridToExport"];
            string idReporte = e.ExtraParams["Reporte"];
            string reportName = idReporte == "P" ? "Pedidos" : "MovimientosLealtad";

            XmlNode gridResultXml = JSON.DeserializeXmlNode("{records:{record:" + gridResultJson + "}}");
            XmlTextReader xtr = new XmlTextReader(gridResultXml.OuterXml, XmlNodeType.Element, null);

            DataSet ds = new DataSet();
            ds.ReadXml(xtr);

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(reportName);

            //Se inserta la tabla completa a la hoja de Excel
            ws.Cell(1, 1).InsertTable(ds.Tables[0].AsEnumerable());

            //Se da el formato deseado a las columnas
            ws = FormatWsColumns(ws, ws.Column(1).CellsUsed().Count(), idReporte);

            //Se prepara la respuesta
            this.Response.Clear();
            this.Response.ClearContent();
            this.Response.ClearHeaders();
            this.Response.Buffer = false;

            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("Content-Disposition", "attachment; filename=" + reportName + ".xlsx");

            //Se envía el reporte como respuesta
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(this.Response.OutputStream);
                memoryStream.Close();
            }

            this.Response.End();
        }

        /// <summary>
        /// Establece el formato deseado a las columnas de la hoja de trabajo por exportar
        /// </summary>
        /// <param name="ws">Hoja de trabajo</param>
        /// <param name="rowsNum">Total de filas de la hoja de trabajo</param>
        /// <param name="reporte">Nemónico identificador del reporte</param>
        /// <returns></returns>
        protected IXLWorksheet FormatWsColumns(IXLWorksheet ws, int rowsNum, string reporte)
        {
            ws.Column(1).Hide();

            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                switch (reporte)
                {
                    case "P":
                        ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
                        ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 9).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 10).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 11).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 12).SetDataType(XLCellValues.Number);
                        break;

                    default:
                        ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
                        ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                        ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                        break;
                }
            }

            return ws;
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar Cambio de Estatus del formulario de Ajustes Manuales,
        /// invocando la actualización del medio de acceso en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarEstatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.hdID_Colectiva.Text != "")
                {
                    LNClientesSmartTicketMC.ActualizaEstatusConfirmaClientes(Convert.ToInt32(cboEstatusConfirma.Value), Convert.ToInt32(this.hdID_Colectiva.Text), this.Usuario);

                    X.Msg.Notify("", "Estatus del Cliente Actualizado <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                    RegistraEnBitacora(Convert.ToInt32(this.hdID_Colectiva.Text), (int)AccionBitacora.CambioEstatus);
                }
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cambio de Estatus", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cambio de Estatus", "Ocurrió un Error al Cambiar el Estatus del Cliente").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Generar Contraseña del formulario de Ajustes Manuales,
        /// invocando la actualización del medio de acceso en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGenerarContrasena_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.hdeMail.Text != "")
                {
                    /*Genera nueva contraseña*/
                    string[] palabras = { "cine", "boletos", "palomitas", "promo", "pelicula" };

                    string contrasena = "";
                    string numeros = "";

                    var seed = Environment.TickCount;
                    var random = new Random(seed);

                    contrasena = palabras[random.Next(0, 4)];

                    for (int i = 1; i <= 5; i++)
                    {
                        numeros = numeros.ToString() + random.Next(0, 9).ToString();
                    }
                    contrasena = contrasena.ToString() + numeros.ToString();

                    LNClientesSmartTicketMC.GenerarContrasenaClientes(Convert.ToInt32(this.hdID_Colectiva.Text), this.hdeMail.Text, contrasena, this.Usuario);
                    txt_Contrasenia.Text = contrasena;

                    X.Msg.Notify("", "Contraseña del Cliente Actualizada <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                    RegistraEnBitacora(Convert.ToInt32(this.hdID_Colectiva.Text), (int)AccionBitacora.CambioContrasena);
                }
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cambio de Contraseña", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cambio de Contraseña", "Ocurrió un Error al Cambiar el Estatus del Cliente").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Aplicar bloqueo o desbloqueo del formulario de Ajustes Manuales,
        /// invocando la actualización del medio de acceso en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAplicarBloqueo_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.hdeMail.Text != "")
                {
                    string leyendaBloqueo;
                    leyendaBloqueo = rdBloqueado.Checked.Equals(true) ? "Bloqueo" : "Desbloqueo";

                    LNClientesSmartTicketMC.AplicaBloqueoContracargoCliente(this.hdeMail.Text, rdBloqueado.Checked, txtComentario.Text, this.Usuario);

                    X.Msg.Notify("", leyendaBloqueo + " del Cliente Aplicado <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                    RegistraEnBitacora(Convert.ToInt32(this.hdID_Colectiva.Text), (int)AccionBitacora.Bloqueo);
                }
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cambio de Contraseña", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Aplicar Bloqueo/Desbloqueo", "Ocurrió un Error al Cambiar el Estatus de Bloqueo/Desbloqueo del Cliente").Show();
            }
        }
    }
}