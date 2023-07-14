using ClosedXML.Excel;
using DALAutorizador.Entidades;
using DALAutorizador.Utilidades;
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
    public partial class ConsultaClientes : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        protected enum AccionBitacora
        {
            Datos = 1,
            Movimientos = 2,
            ActivaTarjeta = 3,
            DesactivaTarjeta = 4,
            Acumulacion = 5,
            Llamada = 6
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
                    //Llenamos el combo de Cadenas Comerciales
                    DataSet dsCadenas = DAOConsultaClientes.ListaCadenasComerciales(
                                 this.Usuario,
                                 Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                    List<ColectivaComboPredictivo> cadenaList = new List<ColectivaComboPredictivo>();

                    foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
                    {
                        var cadenaCombo = new ColectivaComboPredictivo()
                        {
                            ID_Colectiva = Convert.ToInt64(cadena["ID_Colectiva"].ToString()),
                            NombreORazonSocial = cadena["NombreORazonSocial"].ToString()
                        };
                        cadenaList.Add(cadenaCombo);
                    }

                    StoreCadenaComercial.DataSource = cadenaList;
                    StoreCadenaComercial.DataBind();

                    if (dsCadenas.Tables[0].Rows.Count == 1)
                    {
                        this.cBoxCadena.SelectedIndex = 0;
                    }


                    //Prestablecemos las fechas de consulta de los Movimientos
                    dfFechaInicialMov.MaxDate = DateTime.Today;
                    dfFechaInicialMov.SetValue(DateTime.Today.AddDays(-7));


                    dfFechaFinalMov.MaxDate = DateTime.Today;
                    dfFechaFinalMov.SetValue(DateTime.Today);
                    


                    //Se habilitan las pestañas según los roles
                    foreach (string rol in this.Usuario.Roles)
                    {
                        if (rol.Contains("Admin") || rol.Contains("Call") || rol == "PM")
                        {
                            FormPanelAcumulacion.Hidden = false;
                            FormPanelAcumulacion.Show();

                            FormPanelCapturarLlamada.Hidden = false;
                            FormPanelCapturarLlamada.Show();
                        }
                    }

                    FormPanelDatos.Show();
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
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
                Cliente elCliente = new Cliente();

                elCliente.ID_Cadena = String.IsNullOrEmpty(cBoxCadena.SelectedItem.Value) ? 0 : int.Parse(cBoxCadena.SelectedItem.Value);
                elCliente.MedioAcceso = this.txtTarjeta.Text;
                elCliente.ApellidoPaterno = this.txtApPaterno.Text;
                elCliente.ApellidoMaterno = this.txtApMaterno.Text;
                elCliente.Nombre = this.txtNombre.Text;
                elCliente.ID_Cliente = String.IsNullOrEmpty(this.nfIdCliente.Text) ? 0 : int.Parse(this.nfIdCliente.Text);
                elCliente.Email = this.txtCorreo.Text;
                elCliente.FechaNacimiento = Convert.ToDateTime(this.dfFechaNac.Value);

                DataSet dsResultados = LNClientes.ConsultaClientes(elCliente,
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

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
            this.txtColonia.Hidden = true;
            FormPanelDatos.Reset();

            StoreTipoOperacion.RemoveAll();
            StoreTipoCuenta.RemoveAll();
            StoreResultadosMov.RemoveAll();
            FormPanelBuscarMov.Reset();
            dfFechaInicialMov.SetValue(DateTime.Today.AddDays(-7));
            dfFechaFinalMov.SetValue(DateTime.Today);
            btnExportExcel.Disabled = true;

            StoreTarjetas.RemoveAll();
            FormPanelTarjetas.Reset();

            FormPanelAcumulacion.Reset();

            FormPanelCapturarLlamada.Reset();

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

            this.txtColonia.Hidden = true;
            FormPanelDatos.Reset();

            StoreTipoOperacion.RemoveAll();
            StoreTipoCuenta.RemoveAll();
            StoreResultadosMov.RemoveAll();
            FormPanelBuscarMov.Reset();
            dfFechaInicialMov.SetValue(DateTime.Today.AddDays(-7));
            dfFechaFinalMov.SetValue(DateTime.Today);
            btnExportExcel.Disabled = true;
            GridResultadosMov.Title = "Nombre:";

            StoreTarjetas.RemoveAll();
            FormPanelTarjetas.Reset();
            FormPanelTarjetasName.Title = "Nombre:";

            FormPanelAcumulacion.Reset();
            FormPanelDatosAcumulacion.Title = "Nombre:";

            FormPanelCapturarLlamada.Reset();
            FormPanelLlamada.Title = "Nombre:";

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
                int IdCliente = 0;
                int IdMA = 0;

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] cuentaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in cuentaSeleccionada[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Cliente": IdCliente = int.Parse(column.Value); break;
                        case "ID_MA": IdMA = int.Parse(column.Value); break;
                        default:
                            break;
                    }
                }

                limpiaSeleccionPrevia();

                LlenaFieldSetDatosCliente(IdCliente, IdMA);
                LlenaFieldSetBuscarMov();
                LlenaFormPanelTarjetas(IdCliente);
                LlenaFormPanelAcumulacion();
                LlenaFormPanelLlamada();

            }

            catch (CAppException err)
            {
                X.Msg.Alert("Resultados", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Resultados", "Ocurrió un Error al Seleccionar el Cliente").Show();
            }
        }

        /// <summary>
        /// Registra en bitácora la acción realizada en el formulario correspondiente
        /// </summary>
        /// <param name="Id_Cliente">Identificador del cliente</param>
        /// <param name="Formulario">AccionBitacora</param>
        protected void RegistraEnBitacora(int Id_Cliente, int Formulario)
        {
            try
            {
                string Accion;

                switch (Formulario)
                {
                    case (ushort)AccionBitacora.Datos:
                        Accion = "Actualización Datos Cliente";
                        break;

                    case (ushort)AccionBitacora.Movimientos:
                        Accion = "Consulta de Movimientos MDA";
                        break;

                    case (ushort)AccionBitacora.ActivaTarjeta:
                        Accion = "Activación MDA";
                        break;

                    case (ushort)AccionBitacora.DesactivaTarjeta:
                        Accion = "Desactivación MDA";
                        break;

                    case (ushort)AccionBitacora.Acumulacion:
                        Accion = "Acumulación de Operación";
                        break;

                    default:    //case (ushort)AccionBitacora.Llamada
                        Accion = "Registro de Llamada";
                        break;
                }

                DAOBitacoraActividades.InsertaActividad(Id_Cliente, Accion, this.Usuario);
            }

            catch (Exception ex)
            {
                Loguear.Evento("NO se Insertó una Acción en la Bitácora de Actividades del Autorizador de Lealtad. " + ex.Message, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Consulta a BD los datos del cliente seleccionado y los llena en el FieldSet correspondiente
        /// </summary>
        protected void LlenaFieldSetDatosCliente(int idcliente, int idma)
        {
            try
            {
                FormPanelDatos.Reset();
                
                DataSet dsDatos = DAOConsultaClientes.ObtieneDatosCliente(
                    idcliente, 
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                this.txtID_Cliente.Text = idcliente.ToString();
                this.txtID_MA.Text = idma.ToString();

                this.txtNombreCliente.Text      =   dsDatos.Tables[0].Rows[0]["Nombre"].ToString().Trim();
                this.txtApPaternoCliente.Text   =   dsDatos.Tables[0].Rows[0]["ApPaterno"].ToString().Trim();
                this.txtApMaternoCliente.Text   =   dsDatos.Tables[0].Rows[0]["ApMaterno"].ToString().Trim();
                this.txtEmailCliente.Text       =   dsDatos.Tables[0].Rows[0]["Email"].ToString().Trim();
                this.nfTelefonoCliente.Text     =   dsDatos.Tables[0].Rows[0]["Telefono"].ToString().Trim();
                this.dfFechaNacCliente.Value    =   String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(dsDatos.Tables[0].Rows[0]["FechaNacimiento"].ToString()));

                this.txtID_Direccion.Text           =   dsDatos.Tables[0].Rows[0]["ID_DUbicacion"].ToString().Trim();
                this.txtCPCliente.Text              =   dsDatos.Tables[0].Rows[0]["CodigoPostal"].ToString().Trim();
                this.txtIDColonia.Text              =   dsDatos.Tables[0].Rows[0]["ID_Colonia"].ToString().Trim();
                this.cBoxColonia.SelectedItem.Text  =   dsDatos.Tables[0].Rows[0]["Colonia"].ToString().Trim();
                this.txtClaveMunicipio.Text         =   dsDatos.Tables[0].Rows[0]["ClaveMunicipio"].ToString().Trim();
                this.txtMunicipioCliente.Text       =   dsDatos.Tables[0].Rows[0]["Municipio"].ToString().Trim();
                this.txtClaveEstado.Text            =   dsDatos.Tables[0].Rows[0]["ClaveEstado"].ToString().Trim();
                this.txtEstadoCliente.Text          =   dsDatos.Tables[0].Rows[0]["Estado"].ToString().Trim();

                LlenaComboColonias();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Datos del Cliente", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Datos del Cliente", "Ocurrió un Error al Consultar los Datos del Cliente").Show();
            }
        }

        /// <summary>
        /// Llena el combo de colonias y los campos de municipio y estado, con información de base de datos
        /// </summary>
        [DirectMethod(Namespace = "ConsultaClientes")]
        public void LlenaComboColonias()
        {
            if (!String.IsNullOrEmpty(this.txtCPCliente.Text) &&
                this.txtCPCliente.Text.Length >= 5)
            {
                try
                {
                    DataSet dsColonias = DAOConsultaClientes.ListaDatosPorCodigoPostal(
                                this.txtCPCliente.Text,
                                this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.StoreColonias.DataSource = dsColonias;
                    this.StoreColonias.DataBind();

                    this.txtClaveMunicipio.Text = dsColonias.Tables[0].Rows[0]["ClaveMunicipio"].ToString().Trim();
                    this.txtMunicipioCliente.Text = dsColonias.Tables[0].Rows[0]["Municipio"].ToString().Trim();
                    this.txtClaveEstado.Text = dsColonias.Tables[0].Rows[0]["ClaveEstado"].ToString().Trim();
                    this.txtEstadoCliente.Text = dsColonias.Tables[0].Rows[0]["Estado"].ToString().Trim();
                }

                catch (CAppException err)
                {
                    X.Msg.Alert("Datos del Cliente", err.Mensaje()).Show();
                }

                catch (Exception)
                {
                    X.Msg.Alert("Datos del Cliente", "Ocurrió un Error al Consultar los Códigos Postales").Show();
                }
            }
            else
            {
                this.txtClaveMunicipio.Clear();
                this.txtMunicipioCliente.Clear();
                this.txtClaveEstado.Clear();
                this.txtEstadoCliente.Clear();
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
                Cliente cliente = new Cliente();

                cliente.ID_Cliente = int.Parse(this.txtID_Cliente.Text);
                cliente.Nombre = this.txtNombreCliente.Text;
                cliente.ApellidoPaterno = this.txtApPaternoCliente.Text;
                cliente.ApellidoMaterno = this.txtApMaternoCliente.Text;
                cliente.Email = this.txtEmailCliente.Text;
                cliente.Telefono = this.nfTelefonoCliente.Text;
                cliente.FechaNacimiento = Convert.ToDateTime(this.dfFechaNacCliente.Text);

                cliente.IdDireccion = int.Parse(this.txtID_Direccion.Text);
                cliente.IdColonia = this.cBoxColonia.SelectedItem.Value == this.cBoxColonia.SelectedItem.Text ? int.Parse(this.txtIDColonia.Text) : int.Parse(this.cBoxColonia.SelectedItem.Value);
                cliente.Colonia = this.cBoxColonia.SelectedItem.Text == "Otro" ? this.txtColonia.Text : this.cBoxColonia.SelectedItem.Text;
                cliente.CodigoPostal = this.txtCPCliente.Text;
                cliente.ClaveMunicipio = this.txtClaveMunicipio.Text;
                cliente.ClaveEstado = this.txtClaveEstado.Text;

                LNClientes.ActualizaDatosCliente(cliente, this.Usuario);

                X.Msg.Notify("", "Datos del Cliente Actualizados Exitósamente").Show();

                RegistraEnBitacora(cliente.ID_Cliente, (int)AccionBitacora.Datos);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Datos del Cliente", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Datos del Cliente", "Ocurrió un Error al Actualizar los Datos del Cliente").Show();
            }
        }

        /// <summary>
        /// Consulta en base de datos los tipos de operación y cuenta para llenar los combos del FieldSet
        /// de Movimientos.
        /// </summary>
        protected void LlenaFieldSetBuscarMov()
        {
            try
            {
                this.StoreTipoOperacion.DataSource = 
                    DAOConsultaClientes.ListaTiposOperacion (this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                this.StoreTipoOperacion.DataBind();

                this.StoreTipoCuenta.DataSource = 
                    DAOConsultaClientes.ListaTiposCuenta (this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                this.StoreTipoCuenta.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Movimientos", err.Mensaje()).Show();
            }
        }

        /// <summary>
        /// Arma la cadena título de algunos páneles con el nombre del cliente y el saldo actual de su cuenta
        /// </summary>
        /// <returns>Cadena título</returns>
        protected string HeaderPanel(string saldo)
        {
            //"Nombre: \r\n                                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp\r\n                                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp\r\n                                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp\r\n                                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp\r\n                                                Saldo actual:"
            return "Nombre: " + this.txtNombreCliente.Text + " " + this.txtApPaternoCliente.Text + " " + this.txtApMaternoCliente.Text
                    //+ "\r\n                                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp\r\n                                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp\r\n                                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp\r\n                                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp\r\n                                                "
                    + "\r\n                                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp\r\n                                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp\r\n                                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp\r\n                                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp\r\n                                                "
                    + " Saldo actual: " + String.Format("{0:f2}", float.Parse(saldo));
        }

         /// <summary>
        /// Controla el evento Click al botón Buscar del formulario de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarMov_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet dsMovimientos = new DataSet();

                dsMovimientos = LNClientes.ConsultaMovimientos(
                    Convert.ToDateTime(this.dfFechaInicialMov.Text),
                    Convert.ToDateTime(this.dfFechaFinalMov.Text),
                    this.cBoxTipoCuenta.SelectedItem.Value == null ? 0 : int.Parse(this.cBoxTipoCuenta.SelectedItem.Value),
                    this.cBoxTipoOperacion.SelectedItem.Value == null ? 0 : int.Parse(this.cBoxTipoOperacion.SelectedItem.Value),
                    int.Parse(this.txtID_MA.Text), this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (dsMovimientos.Tables[0].Rows.Count > 0)
                {
                    string saldoActual = dsMovimientos.Tables[0].Rows[0]["SaldoActual"].ToString().Trim();
                    this.GridResultadosMov.Title = HeaderPanel(saldoActual);

                    GridResultadosMov.GetStore().DataSource = dsMovimientos;
                    GridResultadosMov.GetStore().DataBind();

                    btnExportExcel.Disabled = false;
                }
                else
                {
                    X.Msg.Alert("Movimientos", "No Existen Movimientos para el Medio de Acceso en el Periodo Solicitado").Show();
                }

                RegistraEnBitacora(int.Parse(this.txtID_Cliente.Text), (int)AccionBitacora.Movimientos);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Movimientos", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Movimientos", "Ocurrió un Error al Buscar losTipos").Show();
            }
        }

        /// <summary>
        /// Extrae el grid de movimientos del backend para generarlo y exportarlo a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            string gridResultJson = e.ExtraParams["GridToExport"];
            string reportName = "Movimientos";

            XmlNode gridResultXml = JSON.DeserializeXmlNode("{records:{record:" + gridResultJson + "}}");
            XmlTextReader xtr = new XmlTextReader(gridResultXml.OuterXml, XmlNodeType.Element, null);

            DataSet ds = new DataSet();
            ds.ReadXml(xtr);

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(reportName);

            //Se inserta la tabla completa a la hoja de Excel
            ws.Cell(1, 1).InsertTable(ds.Tables[0].AsEnumerable());

            //Se da el formato deseado a las columnas
            ws = FormatWsColumns(ws, ws.Column(1).CellsUsed().Count());

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
        /// <returns></returns>
        protected IXLWorksheet FormatWsColumns(IXLWorksheet ws, int rowsNum)
        {
            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 9).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 10).SetDataType(XLCellValues.Number);
            }

            return ws;
        }


        /// <summary>
        /// Consulta en base de datos los medios de acceso del cliente seleccionado y los 
        /// llena en el FormPanel de tarjetas
        /// </summary>
        protected void LlenaFormPanelTarjetas(int idcliente)
        {
            try
            {
                FormPanelTarjetas.Reset();

                int Id_Cliente = idcliente == 0 ? int.Parse(this.txtID_Cliente.Text) : idcliente;

                this.StoreTarjetas.DataSource = DAOConsultaClientes.ObtieneMediosAccesoCliente(
                    Id_Cliente, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                this.StoreTarjetas.DataBind();

                this.FormPanelTarjetasName.Title = "Nombre: " + this.txtNombreCliente.Text + " " + this.txtApPaternoCliente.Text + " " + this.txtApMaternoCliente.Text;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tarjetas", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Tarjetas", "Ocurrió un Error al Consultar los Medios de Acceso del Cliente").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de tarjetas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            String EjecutarComando = (String)e.ExtraParams["Comando"];
            //Boolean activo = Convert.ToBoolean(e.ExtraParams["Activo"]);
            //this.hidActivo.Value = activo;
            int idEst = Convert.ToInt32(e.ExtraParams["IdEstatus"]);
            this.hidIdEstatus.Value = idEst;

            switch (EjecutarComando)
            {
                //case "Activar":
                case "Desactivar":
                    //X.Msg.Confirm("Tarjetas", "¿Estás seguro de " + (activo ? "desactivar" : "activar") + " la tarjeta seleccionada?",
                    X.Msg.Confirm("Tarjetas", "¿Estás seguro de cancelar la tarjeta seleccionada?",
                        new Ext.Net.MessageBoxButtonsConfig
                    {
                        Yes = new Ext.Net.MessageBoxButtonConfig
                        {
                            Handler = "Loyalty.muestraCapturaRazon()",
                            Text = "Aceptar"
                        },
                        No = new Ext.Net.MessageBoxButtonConfig
                        {
                            Text = "Cancelar"
                        }
                    }).Show(); 
                    break;

                default: break;
            }
        }

        /// <summary>
        /// Controla el evento clic al botón Aceptar a la confirmación de desactivación de la tarjeta.
        /// Muestra la ventana de diálogo con las 
        /// </summary>
        [DirectMethod(Namespace = "Loyalty")]
        public void muestraCapturaRazon()
        {
            this.WindowEstatusTarjetaRazon.Hidden = false;
            this.WindowEstatusTarjetaRazon.Show();

            //#WARNING: POner titulo dinamico del DIALOG??
        }

        /// <summary>
        /// Controla el evento clic al botón Guardar de la beventana de diálogo que
        /// solicita las razones de la cancelación de la tarjeta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardaRazonEstatusTarjeta_Click(object sender, EventArgs e)
        {
            try
            {
                this.WindowEstatusTarjetaRazon.Hidden = true;

                //bool activo = Convert.ToBoolean(this.hidActivo.Value);
                int Id_Estatus = Convert.ToInt32(this.hidIdEstatus.Value);

                LNClientes.ActualizaEstatusMA(int.Parse(this.txtID_MA.Text), Id_Estatus, this.txtRazones.Text, this.Usuario);
                X.Msg.Notify("", "Medio de Acceso Desactivado Exitósamente").Show();
                //X.Msg.Notify("", "Medio de Acceso " + (activo ? "Desactivado" : "Activado") + " Exitósamente").Show()

                //RegistraEnBitacora(int.Parse(this.txtID_Cliente.Text), (activo ? (int)AccionBitacora.DesactivaTarjeta : (int)AccionBitacora.ActivaTarjeta));
                RegistraEnBitacora(int.Parse(this.txtID_Cliente.Text), (int)AccionBitacora.DesactivaTarjeta);

                LlenaFormPanelTarjetas(0);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tarjetas", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Tarjetas", "Ocurrió un Error al Activar/Inactivar el Medio de Acceso del Cliente").Show();
            }
        }

        /// <summary>
        /// Da el título dinámico al panel de acumulación con el nombre del cliente seleccionado
        /// </summary>
        protected void LlenaFormPanelAcumulacion()
        {
            FormPanelDatosAcumulacion.Reset();

            this.FormPanelDatosAcumulacion.Title = "Nombre: " + this.txtNombreCliente.Text + " " + this.txtApPaternoCliente.Text + " " + this.txtApMaternoCliente.Text;
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario de acumulación, limpiando los controles
        /// asociados a éste
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarAcum_Click(object sender, EventArgs e)
        {
            FormPanelDatosAcumulacion.Reset();
        }

        /// <summary>
        /// Consulta en base de datos el cátálogo de actividades para el motivo de la llamada 
        /// para llenar el combo correspondiente dentro del FieldSetCapturarLlamada
        /// </summary>
        protected void LlenaFormPanelLlamada()
        {
            try
            {
                this.StoreMotivos.DataSource = DAOConsultaClientes.ListaMotivosLlamada(this.Usuario);
                this.StoreMotivos.DataBind();

                this.FormPanelLlamada.Title = "Nombre: " + this.txtNombreCliente.Text + " " + this.txtApPaternoCliente.Text + " " + this.txtApMaternoCliente.Text;
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Capturar Llamada", err.Mensaje()).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del formulario de captura de llamada,
        /// invocando la inserción del refistro correspondiente en la bitácora de base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarLlamada_Click(object sender, EventArgs e)
        {
            try
            {
                LNClientes.RegistraLlamadaCliente(
                    int.Parse(this.txtID_Cliente.Text),
                    int.Parse(this.cBoxMotivoLlamada.SelectedItem.Value),
                    this.txtComentarios.Text, this.Usuario);

                X.Msg.Notify("", "La Llamada se Capturó Exitósamente").Show();

                RegistraEnBitacora(int.Parse(this.txtID_Cliente.Text), (int)AccionBitacora.Llamada);

                FormPanelCapturarLlamada.Reset();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Captura de Llamada", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Captura de Llamada", "Ocurrió un Error al Registrar la Llamada del Cliente").Show();
            }
        }
    }
}