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
    public partial class ConsultaClientesMoshi : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        protected enum AccionBitacora
        {
            Datos = 1,
            CambioNivel = 2,
            CargaPuntos = 3,
            AbonoPuntos = 4,
            CambioEstatus = 5
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
                    //Llenamos el combo de niveles de lealtad
                    StoreNivelesLealtad.DataSource = DAOConsultaClientesMoshi.ObtieneNivelesLealtad(this.Usuario);
                    StoreNivelesLealtad.DataBind();

                    var empresasSource = DAOConsultaClientesMoshi.GetEmpresas(this.Usuario,Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreEmpresas.DataSource = empresasSource;
                    StoreEmpresas.DataBind();

                    //Prestablecemos los valores de estatus de medios de acceso
                    DataSet dsEstatusMA = DAOConsultaClientesMoshi.ObtieneEstatusMediosAcceso(this.Usuario);
                    rdActivo.InputValue = dsEstatusMA.Tables[0].Rows[0]["ID_EstatusMA"].ToString();
                    rdInactivo.InputValue = dsEstatusMA.Tables[0].Rows[1]["ID_EstatusMA"].ToString();

                    //Prestablecemos las fechas de consulta de los Pedidos
                    dfFechaInicialPed.MaxDate = DateTime.Today;
                    dfFechaInicialPed.SetValue(DateTime.Today.AddDays(-30));

                    dfFechaFinalPed.MaxDate = DateTime.Today;
                    dfFechaFinalPed.SetValue(DateTime.Today);

                   
                    //Prestablecemos las fechas de consulta de los Movimientos de Lealtad
                    dfFechaInicialMovLeal.MaxDate = DateTime.Today;
                    dfFechaInicialMovLeal.SetValue(DateTime.Today.AddDays(-30));

                    dfFechaFinalMovLeal.MaxDate = DateTime.Today;
                    dfFechaFinalMovLeal.SetValue(DateTime.Today);
                  

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
                
                DataSet dsResultados = LNMoshi.ConsultaClientes(elCliente, this.Usuario);

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
            btnGuardaDatos.Disabled = true;

            FormPanelDirecciones.Reset();

            StoreResultadosPed.RemoveAll();
           // FormPanelBuscarPedidos.Reset();
            dfFechaInicialPed.SetValue(DateTime.Today.AddDays(-30));
            dfFechaFinalPed.SetValue(DateTime.Today);
            btnGuardaDatos.Disabled = true;
            btnExcelPed.Disabled = true;

            StoreResultadosMovLeal.RemoveAll();
            FormPanelBuscarMovLeal.Reset();
            GridResultadosMovLeal.Title = "Saldo:";
            dfFechaInicialMovLeal.SetValue(DateTime.Today.AddDays(-30));
            dfFechaFinalMovLeal.SetValue(DateTime.Today);
            btnExcelMovLeal.Disabled = true;

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

            FormPanelDirecciones.Reset();

            StoreResultadosPed.RemoveAll();
           // FormPanelBuscarPedidos.Reset();
            dfFechaInicialPed.SetValue(DateTime.Today.AddDays(-30));
            dfFechaFinalPed.SetValue(DateTime.Today);

            StoreResultadosMovLeal.RemoveAll();
            FormPanelBuscarMovLeal.Reset();
            GridResultadosMovLeal.Title = "Saldo:";
            dfFechaInicialMovLeal.SetValue(DateTime.Today.AddDays(-30));
            dfFechaFinalMovLeal.SetValue(DateTime.Today);

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
                String eMailCliente =  "", EstatusMA = "";

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
                LlenaComboAliasDirecciones(IdCliente);
                LlenaFieldSetEstatus(IdColectiva, EstatusMA);

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
        protected void RegistraEnBitacora(int IdColectiva, int Formulario, String Observaciones)
        {
            try
            {
                string Accion;

                switch (Formulario)
                {
                    case (ushort)AccionBitacora.Datos:
                        Accion = "Actualización Datos Cliente Moshi-Moshi";
                        break;

                    case (ushort)AccionBitacora.CambioNivel:
                        Accion = "Ajuste Manual Nivel Moshi-Moshi";
                        break;

                    case (ushort)AccionBitacora.CargaPuntos:
                        Accion = "Ajuste Manual Carga de Puntos Moshi-Moshi";
                        break;

                    case (ushort)AccionBitacora.AbonoPuntos:
                        Accion = "Ajuste Manual Abono de Puntos Moshi-Moshi";
                        break;

                    default:    //case (ushort)AccionBitacora.CambioEstatus
                        Accion = "Ajuste Manual Cambio de Estatus Moshi-Moshi";
                        break;
                }

                DAOBitacoraActividades.InsertaActividadMoshiEnBitacora(IdColectiva, Accion, Observaciones, this.Usuario);
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
                btnGuardaDatos.Disabled = false;

                DataSet dsDatos = DAOConsultaClientesMoshi.ObtieneDatosCliente(idcliente, idcolectiva, this.Usuario);

                this.hdID_Cliente.Text = idcliente.ToString();
                this.hdID_Colectiva.Text = idcolectiva.ToString();
                this.hdID_TipoColectiva.Text = idtipocolectiva.ToString();
                this.hdeMail.Text = eMail;
               
                this.txtNombreCliente.Text = dsDatos.Tables[0].Rows[0]["Nombre"].ToString().Trim();
                this.txtApPaternoCliente.Text = dsDatos.Tables[0].Rows[0]["ApPaterno"].ToString().Trim();
                this.txtApMaternoCliente.Text = dsDatos.Tables[0].Rows[0]["ApMaterno"].ToString().Trim();

                this.txtEmailCliente.Text = dsDatos.Tables[0].Rows[0]["Email"].ToString().Trim();
                this.dfFechaNacCliente.Value = String.Format("{0:dd/MM/yyyy}", 
                    Convert.ToDateTime(dsDatos.Tables[0].Rows[0]["FechaNacimiento"].ToString()));

                this.txtVisitas.Text = dsDatos.Tables[0].Rows[0]["Visitas"].ToString();
                this.dfFechaAlta.Value = String.Format("{0:dd/MM/yyyy}",
                    Convert.ToDateTime(dsDatos.Tables[0].Rows[0]["FechaAlta"].ToString()));
                this.dfFechaAltaLealtad.Value = 
                    String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["FechaAltaLealtad"].ToString()) ? "" : 
                    String.Format("{0:dd/MM/yyyy}", 
                    Convert.ToDateTime(dsDatos.Tables[0].Rows[0]["FechaAltaLealtad"].ToString()));

                this.txtNivelLealtad.Text = dsDatos.Tables[0].Rows[0]["NivelLealtad"].ToString().Trim();

                this.txtcorreoEmpresa.Text = dsDatos.Tables[0].Rows[0]["correoEmpresa"].ToString().Trim();


                this.ComboEmpresa.Value = dsDatos.Tables[0].Rows[0]["ClaveEmpresa"].ToString().Trim();

                this.txtlugarTrabajo.Text = dsDatos.Tables[0].Rows[0]["lugarTrabajo"].ToString().Trim();



                //Establecemos seleccionado por default el nivel de lealtad del cliente
                this.cBoxNivel.SelectedItem.Value = dsDatos.Tables[0].Rows[0]["IdNivelLealtad"].ToString().Trim();
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
        /// Controla el evento Click al botón Guardar del formulario de Datos, invocando la actualización
        /// de los datos del cliente en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardaDatos_Click(object sender, EventArgs e)
        {
            try
            {
                ClienteColectiva cliente = new ClienteColectiva();

                cliente.ID_Cliente = int.Parse(this.hdID_Cliente.Text);
                cliente.Nombre = this.txtNombreCliente.Text;
                cliente.ApellidoPaterno = this.txtApPaternoCliente.Text;
                cliente.ApellidoMaterno = this.txtApMaternoCliente.Text;
                cliente.FechaNacimiento = Convert.ToDateTime(this.dfFechaNacCliente.Text);


                cliente.CorreoEmpresa = this.txtcorreoEmpresa.Text;

                cliente.ClaveEmpresa = this.ComboEmpresa.SelectedItem.Value;

                cliente.LugarTrabajo= this.txtlugarTrabajo.Text;

                LNMoshi.ActualizaDatosCliente(cliente, this.Usuario);

                X.Msg.Notify("", "Datos del Cliente Actualizados <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                RegistraEnBitacora(cliente.ID_Colectiva, (int)AccionBitacora.Datos, "");
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos del Cliente", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos del Cliente", "Ocurrió un Error al Actualizar los Datos del Cliente").Show();
            }
        }

        /// <summary>
        /// Consulta a BD las direcciones del cliente seleccionado, llenando el Combo correspondiente
        /// </summary>
        protected void LlenaComboAliasDirecciones(int idcliente)
        {
            try
            {
                StoreAliasDirecciones.RemoveAll();

                DataSet dsAlias = DAOConsultaClientesMoshi.ObtieneAliasDireccionesCliente(
                            idcliente, this.Usuario);

                this.StoreAliasDirecciones.DataSource = dsAlias;
                this.StoreAliasDirecciones.DataBind();
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Alias de Direcciones", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Alias de Direcciones", "Ocurrió un Error al Consultar los Alias de Direcciones del Cliente").Show();
            }
        }

        /// <summary>
        /// Llena el FieldSet del Estatus del Medio de Acceso con la información del cliente
        /// </summary>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="EstatusMA">Estatus del Medio de Acceso</param>
        protected void LlenaFieldSetEstatus(int idColectiva, String EstatusMA)
        {
            try
            {
                txtMotivo.Text = DAOConsultaClientesMoshi.ConsultaMotivoEstatusMA(idColectiva, this.Usuario);

                rdActivo.Checked = EstatusMA.Equals("ACTIVA") ? true : false;
                rdInactivo.Checked = EstatusMA.Equals("INACTIVA") ? true : false;

                if (hdnPageLoad.Text == "1")
                {
                    hdnPageLoad.Text = "0";
                    FieldSetEstatus.Render();
                }
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Ajustes Manuales", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Ajustes Manuales", "Ocurrió un Error al Consultar el Estatus del Medio de Acceso del Cliente").Show();
            }
        }


        /// <summary>
        /// Consulta a BD los datos de la dirección del cliente, llenando el FieldSet correspondiente
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void LlenaFieldSetDirecciones(object sender, EventArgs e)
        {
            try
            {
                DataSet dsDirecciones = 
                    DAOConsultaClientesMoshi.ObtieneDireccionesCliente(
                    Convert.ToInt32(this.cBoxAliasDireccion.SelectedItem.Value), this.Usuario);

                this.txtCalle.Text = dsDirecciones.Tables[0].Rows[0]["Calle"].ToString().Trim();
                this.txtNumeroExterior.Text = dsDirecciones.Tables[0].Rows[0]["NumExterior"].ToString().Trim();
                this.txtNumeroInterior.Text = dsDirecciones.Tables[0].Rows[0]["NumInterior"].ToString().Trim();
                this.txtCodigoPostal.Text = dsDirecciones.Tables[0].Rows[0]["CodigoPostal"].ToString().Trim();
                this.txtColonia.Text = dsDirecciones.Tables[0].Rows[0]["Colonia"].ToString().Trim();
                this.txtEntreCalles.Text = dsDirecciones.Tables[0].Rows[0]["EntreCalles"].ToString().Trim();
                this.txtReferencias.Text = dsDirecciones.Tables[0].Rows[0]["Referencias"].ToString().Trim();
                this.txtTelefono.Text = dsDirecciones.Tables[0].Rows[0]["Telefono"].ToString().Trim();
                this.txtExtension.Text = dsDirecciones.Tables[0].Rows[0]["Extension"].ToString().Trim();
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Direcciones del Cliente", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Direcciones del Cliente", "Ocurrió un Error al Consultar las Direcciones del Cliente").Show();
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


                dsPedidos = LNMoshi.ConsultaPedidos(
                    elClienteSeleccionado,
                    //Convert.ToDateTime(this.dfFechaInicialPed.SelectedDate),
                    //Convert.ToDateTime(this.dfFechaFinalPed.SelectedDate),
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
        /// Controla el evento Click al botón Buscar del formulario de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarMovLeal_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.hdID_Colectiva.Text))
                {
                    return;
                }

                StoreResultadosMovLeal.RemoveAll();

                DataSet dsMovimientos = new DataSet();

                dsMovimientos = LNMoshi.ConsultaMovimientos(
                    Convert.ToDateTime(this.dfFechaInicialMovLeal.SelectedDate),
                    Convert.ToDateTime(this.dfFechaFinalMovLeal.SelectedDate),
                    this.hdID_Colectiva.Text == "" ? 0 : Convert.ToInt32(this.hdID_Colectiva.Text),
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));


                if (dsMovimientos.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Movimientos", "No Existen Movimientos de Lealtad del Cliente en el Periodo Indicado.").Show();
                }

                else
                {
                    this.GridResultadosMovLeal.Title = "Saldo: " + String.Format("{0:f2}",
                        float.Parse(dsMovimientos.Tables[0].Rows[0]["SaldoActual"].ToString().Trim()));

                    StoreResultadosMovLeal.DataSource = dsMovimientos;
                    StoreResultadosMovLeal.DataBind();

                    btnExcelMovLeal.Disabled = false;
                }
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Movimientos de Lealtad", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Movimientos de Lealtad", "Ocurrió un Error al Buscar los Pedidos").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar Nivel de Lealtad del formulario de Ajustes Manuales,
        /// invocando la actualización de dicho nivel en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarNivel_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.hdID_Colectiva.Text) ||
                    String.IsNullOrEmpty(this.cBoxNivel.SelectedItem.Value))
                {
                    return;
                }

                int Id_Colectiva = Convert.ToInt32(this.hdID_Colectiva.Text);
                int Id_NivelLealtad = Convert.ToInt32(this.cBoxNivel.SelectedItem.Value);

                LNMoshi.ActualizaNivelLealtadCliente(Id_Colectiva, Id_NivelLealtad, this.Usuario);

                X.Msg.Notify("", "Nivel de Lealtad del Cliente Actualizado <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                RegistraEnBitacora(Id_Colectiva, (int)AccionBitacora.CambioNivel, "Nuevo [ID_GrupoCuenta] = " + Id_NivelLealtad.ToString());
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Nivel de Lealtad", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Nivel de Lealtad", "Ocurrió un Error al Actualizar el Nivel de Lealtad del Cliente").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Aplicar para la Carga de Puntos Manual
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnAplicaCarga_Click(object sender, DirectEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.nfImporteCarga.Text) &&
                    String.IsNullOrEmpty(this.txtObsCargaPts.Text))
                {
                    return;
                }

                if (Convert.ToDecimal(this.nfImporteCarga.Text) == 0)
                {
                    X.Msg.Alert("Ajustes Manuales", "Captura el Importe de la Carga de Puntos").Show();
                    return;
                }

                EventosManuales elEvento = new EventosManuales();

                DataSet dsEvento = DAOConsultaClientesMoshi.ConsultaEventoAjusteCargo
                    (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                elEvento.IdEvento = Convert.ToInt32(dsEvento.Tables[0].Rows[0]["ID_Evento"].ToString());
                elEvento.ClaveEvento = dsEvento.Tables[0].Rows[0]["ClaveEvento"].ToString();
                elEvento.Concepto = dsEvento.Tables[0].Rows[0]["Descripcion"].ToString();
                elEvento.IdColectiva = Convert.ToInt64(this.hdID_Colectiva.Text);
                elEvento.IdTipoColectiva = Convert.ToInt32(this.hdID_TipoColectiva.Text);
                elEvento.ClaveCadenaComercial = Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveCadenaMoshi").Valor;
                elEvento.Importe = this.nfImporteCarga.Text;
                elEvento.Observaciones = this.txtObsCargaPts.Text;

                LNMoshi.RegistraEvManual_AjusteCargo(elEvento, this.Usuario);

                X.Msg.Notify("Ajustes Manuales", "Carga de Puntos <br /><br />  <b> E X I T O S A </b> <br />  <br /> ").Show();

                this.FormPanelAjustesManuales.Reset();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Carga de Puntos", err.Mensaje()).Show();
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Carga de Puntos", "Ocurrió un Error al Cargar los Puntos").Show();
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Aplicar para el Abono de Puntos Manual
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnAplicaAbono_Click(object sender, DirectEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.nfImporteAbono.Text) &&
                    String.IsNullOrEmpty(this.txtObsAbonoPts.Text))
                {
                    return;
                }

                if (Convert.ToDecimal(this.nfImporteAbono.Text) == 0)
                {
                    X.Msg.Alert("Ajustes Manuales", "Captura el Importe del Abono de Puntos").Show();
                    return;
                }

                EventosManuales elEvento = new EventosManuales();

                DataSet dsEvento = DAOConsultaClientesMoshi.ConsultaEventoAjusteAbono
                    (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                elEvento.IdEvento = Convert.ToInt32(dsEvento.Tables[0].Rows[0]["ID_Evento"].ToString());
                elEvento.ClaveEvento = dsEvento.Tables[0].Rows[0]["ClaveEvento"].ToString();
                elEvento.Concepto = dsEvento.Tables[0].Rows[0]["Descripcion"].ToString();
                elEvento.IdColectiva = Convert.ToInt64(this.hdID_Colectiva.Text);
                elEvento.IdTipoColectiva = Convert.ToInt32(this.hdID_TipoColectiva.Text);
                elEvento.ClaveCadenaComercial = Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveCadenaMoshi").Valor;
                elEvento.Importe = this.nfImporteAbono.Text;
                elEvento.Observaciones = this.txtObsAbonoPts.Text;

                LNMoshi.RegistraEvManual_AjusteAbono(elEvento, this.Usuario);

                X.Msg.Notify("Ajustes Manuales", "Abono de Puntos <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();

                this.FormPanelAjustesManuales.Reset();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Abono de Puntos", err.Mensaje()).Show();
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Abono de Puntos", "Ocurrió un Error al Abonar los Puntos").Show();
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Aplicar de la Carga de Visitas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnAplicaCargaV_Click(object sender, DirectEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.nfCargaVisitas.Text) &&
                    String.IsNullOrEmpty(this.txtObsCargaV.Text))
                {
                    return;
                }

                if (Convert.ToDecimal(this.nfCargaVisitas.Text) == 0)
                {
                    X.Msg.Alert("Ajustes Manuales", "Captura el Número de Visitas de la Carga de Visitas").Show();
                    return;
                }

                EventosManuales elEvento = new EventosManuales();

                DataSet dsEvento = DAOConsultaClientesMoshi.ConsultaEventoAjusteCargoVisitas
                    (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                elEvento.IdEvento = Convert.ToInt32(dsEvento.Tables[0].Rows[0]["ID_Evento"].ToString());
                elEvento.ClaveEvento = dsEvento.Tables[0].Rows[0]["ClaveEvento"].ToString();
                elEvento.Concepto = dsEvento.Tables[0].Rows[0]["Descripcion"].ToString();
                elEvento.IdColectiva = Convert.ToInt64(this.hdID_Colectiva.Text);
                elEvento.IdTipoColectiva = Convert.ToInt32(this.hdID_TipoColectiva.Text);
                elEvento.ClaveCadenaComercial = Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveCadenaMoshi").Valor;
                elEvento.Importe = this.nfCargaVisitas.Text;
                elEvento.Observaciones = this.txtObsCargaV.Text;

                LNMoshi.RegistraEvManual_AjusteCargoVisitas(elEvento, this.Usuario);

                X.Msg.Notify("Ajustes Manuales", "Carga de Visitas <br /><br />  <b> E X I T O S A </b> <br />  <br /> ").Show();

                this.FormPanelAjustesManuales.Reset();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Carga de Visitas", err.Mensaje()).Show();
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Carga de Visitas", "Ocurrió un Error al Cargar las Visitas").Show();
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Aplicar para el Abono de Puntos Manual
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnAplicaAbonoV_Click(object sender, DirectEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.nfAbonoVisitas.Text) &&
                    String.IsNullOrEmpty(this.txtObsAbonoV.Text))
                {
                    return;
                }

                if (Convert.ToDecimal(this.nfAbonoVisitas.Text) == 0)
                {
                    X.Msg.Alert("Ajustes Manuales", "Captura el Número de Visitas del Abono de Visitas").Show();
                    return;
                }

                EventosManuales elEvento = new EventosManuales();

                DataSet dsEvento = DAOConsultaClientesMoshi.ConsultaEventoAjusteAbonoVisitas
                    (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                elEvento.IdEvento = Convert.ToInt32(dsEvento.Tables[0].Rows[0]["ID_Evento"].ToString());
                elEvento.ClaveEvento = dsEvento.Tables[0].Rows[0]["ClaveEvento"].ToString();
                elEvento.Concepto = dsEvento.Tables[0].Rows[0]["Descripcion"].ToString();
                elEvento.IdColectiva = Convert.ToInt64(this.hdID_Colectiva.Text);
                elEvento.IdTipoColectiva = Convert.ToInt32(this.hdID_TipoColectiva.Text);
                elEvento.ClaveCadenaComercial = Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveCadenaMoshi").Valor;
                elEvento.Importe = this.nfAbonoVisitas.Text;
                elEvento.Observaciones = this.txtObsAbonoV.Text;

                LNMoshi.RegistraEvManual_AjusteAbonoVisitas(elEvento, this.Usuario);

                X.Msg.Notify("Ajustes Manuales", "Abono de Visitas <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();

                this.FormPanelAjustesManuales.Reset();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Abono de Visitas", err.Mensaje()).Show();
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Abono de Visitas", "Ocurrió un Error al Abonar las Visitas").Show();
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
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
                if (String.IsNullOrEmpty(this.hdeMail.Text))
                {
                    return;
                }

                if (String.IsNullOrEmpty(this.txtMotivo.Text))
                {
                    X.Msg.Alert("Cambio de Estatus", "El motivo de cambio de estatus es obligatorio").Show();
                    return;
                }

                int idEstatus = this.rdActivo.Checked ?
                    Convert.ToInt32(this.rdActivo.InputValue) : Convert.ToInt32(this.rdInactivo.InputValue);

                LNMoshi.ActualizaEstatusMA(this.hdeMail.Text, idEstatus, this.Usuario);

                X.Msg.Notify("", "Estatus del Cliente Actualizado <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                RegistraEnBitacora(Convert.ToInt32(this.hdID_Colectiva.Text), (int)AccionBitacora.CambioEstatus, this.txtMotivo.Text);
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
    }
}