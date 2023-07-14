using ClosedXML.Excel;
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
    public partial class ConsultaClientesTeleVIP : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        protected enum AccionBitacora
        {
            Datos = 1,
            Movimientos = 2,
            Recompensas = 3
            //Llamada = 4
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
                    lblAcumuladoPeriodo.Style.Add("text-align", "right");

                    //Prestablecemos las fechas de consulta de los Movimientos
                    dfFechaInicialMov.SetValue(DateTime.Today.AddDays(-7));
                    dfFechaInicialMov.MaxDate = DateTime.Today;

                    dfFechaFinalMov.SetValue(DateTime.Today);
                    dfFechaFinalMov.MaxDate = DateTime.Today;

                    ////Se habilitan las pestañas según los roles
                    //foreach (string rol in this.Usuario.Roles)
                    //{
                    //    if (rol.Contains("Admin") || rol.Contains("Call"))
                    //    {
                    //        FormPanelAcumulacion.Hidden = false;
                    //        FormPanelAcumulacion.Show();

                    //        FormPanelCapturarLlamada.Hidden = false;
                    //        FormPanelCapturarLlamada.Show();
                    //    }
                    //}
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

                elCliente.ApellidoPaterno = this.txtApPaterno.Text;
                elCliente.ApellidoMaterno = this.txtApMaterno.Text;
                elCliente.Nombre = this.txtNombre.Text;
                elCliente.ID_Cliente = String.IsNullOrEmpty(this.nfIdCliente.Text) ? 0 : int.Parse(this.nfIdCliente.Text);
                elCliente.ID_Cuenta = String.IsNullOrEmpty(this.nfCuenta.Text) ? 0 : int.Parse(this.nfCuenta.Text);
                elCliente.MedioAcceso = this.txtTag.Text;
                elCliente.Email = this.txtCorreo.Text;

                DataSet dsResultados = LNClientesTeleVIP.ConsultaClientes(elCliente,
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
            StoreTagsPorCuenta.RemoveAll();
            cBoxTags.SetValue("");
            cBoxTags.SelectedIndex = -1;
            StoreResultadosMov.RemoveAll();

            FormPanelMovsAcumulados.Reset();
            this.lblMes1.Text = "";
            this.lblMes2.Text = "";
            this.lblMes3.Text = "";
            this.lblAcumMes1.Text = "0.00";
            this.lblAcumMes2.Text = "0.00";
            this.lblAcumMes3.Text = "0.00";
            this.lblAcumuladoPeriodo.Text = "";

            FormPanelBuscarMov.Reset();
            dfFechaInicialMov.SetValue(DateTime.Today.AddDays(-7));
            dfFechaFinalMov.SetValue(DateTime.Today);

            StoreTagsRec.RemoveAll();
            cBoxTagsBuscarRec.SetValue("");
            cBoxTagsBuscarRec.SelectedIndex = -1;
            StoreResultadosRec.RemoveAll();
            FormPanelRecompensas.Reset();

            //FormPanelCapturarLlamada.Reset();

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
            this.txtID_Cliente.Text = "";
            this.txtID_MA.Text = "";
            this.txtCuenta.Text = "";
            this.lblNombreCliente.Text = "";
            this.lblApPaternoCliente.Text = "";
            this.lblApMaternoCliente.Text = "";
            this.lblEmailCliente.Text = "";
            this.lblTelefonoCliente.Text = "";
            this.lblFechaNacCliente.Text = "";
            this.lblCPCliente.Text = "";
            this.lblColonia.Text = "";
            this.lblEstadoCliente.Text = "";
            this.lblFechaRegCliente.Text = "";

            StoreTagsPorCuenta.RemoveAll();
            cBoxTags.SetValue("");
            cBoxTags.SelectedIndex = -1;
            StoreResultadosMov.RemoveAll();

            FormPanelMovsAcumulados.Reset();
            this.lblMes1.Text = "";
            this.lblMes2.Text = "";
            this.lblMes3.Text = "";
            this.lblAcumMes1.Text = "0.00";
            this.lblAcumMes2.Text = "0.00";
            this.lblAcumMes3.Text = "0.00";
            this.lblAcumuladoPeriodo.Text = "";

            FormPanelBuscarMov.Reset();
            dfFechaInicialMov.SetValue(DateTime.Today.AddDays(-7));
            dfFechaFinalMov.SetValue(DateTime.Today);
            GridResultadosMov.Title = "Nombre:";
            btnExportExcel.Disabled = true;

            StoreTagsRec.RemoveAll();
            cBoxTagsBuscarRec.SetValue("");
            cBoxTagsBuscarRec.SelectedIndex = -1;
            StoreResultadosRec.RemoveAll();
            FormPanelRecompensas.Reset();
            GridResultadosRec.Title = "Nombre:";

            //FormPanelCapturarLlamada.Reset();
            //FormPanelLlamada.Title = "Nombre:";

            FormPanelDatos.Show();
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario de búsqueda
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
                string Cuenta = "";

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] cuentaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in cuentaSeleccionada[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Cliente": IdCliente = int.Parse(column.Value); break;
                        case "ID_MA": IdMA = int.Parse(column.Value); break;
                        case "ID_Cuenta": Cuenta = column.Value; break;
                        default:
                            break;
                    }
                }

                limpiaSeleccionPrevia();

                LlenaFieldSetDatosCliente(IdCliente, IdMA, Cuenta);
                LlenaFieldSetBuscarMov();
                LlenaFormPanelRecompensas();
                //LlenaFormPanelLlamada();

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
                        Accion = "Consulta de Datos Cliente TVIP";
                        break;

                    case (ushort)AccionBitacora.Movimientos:
                        Accion = "Consulta de Movimientos Tag TVIP";
                        break;

                    default: //case (ushort)AccionBitacora.Recompensas:
                        Accion = "Consulta de Recompensas TVIP";
                        break;


                    //default:    //case (ushort)AccionBitacora.Llamada
                    //    Accion = "Registro de Llamada";
                    //    break;
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
        protected void LlenaFieldSetDatosCliente(int idcliente, int idma, string cuenta)
        {
            try
            {
                FormPanelDatos.Reset();
                
                DataSet dsDatos = DAOConsultaClientesTeleVIP.ObtieneDatosCliente(
                    idcliente, 
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                this.txtID_Cliente.Text = idcliente.ToString();
                this.txtID_MA.Text = idma.ToString();
                this.txtCuenta.Text = cuenta;

                this.lblNombreCliente.Text      =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["Nombre"].ToString().Trim()) ? "_" : dsDatos.Tables[0].Rows[0]["Nombre"].ToString().Trim();
                this.lblApPaternoCliente.Text   =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["ApPaterno"].ToString().Trim()) ? "_" : dsDatos.Tables[0].Rows[0]["ApPaterno"].ToString().Trim();
                this.lblApMaternoCliente.Text   =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["ApMaterno"].ToString().Trim()) ? "_" : dsDatos.Tables[0].Rows[0]["ApMaterno"].ToString().Trim();

                this.GridResultadosMov.Title = "Nombre: " + this.lblNombreCliente.Text + " " + this.lblApPaternoCliente.Text + " " + this.lblApMaternoCliente.Text;
                this.GridResultadosRec.Title = "Nombre: " + this.lblNombreCliente.Text + " " + this.lblApPaternoCliente.Text + " " + this.lblApMaternoCliente.Text;

                this.lblEmailCliente.Text       =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["Email"].ToString().Trim()) ? "_" : dsDatos.Tables[0].Rows[0]["Email"].ToString().Trim();
                this.lblTelefonoCliente.Text    =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["Telefono"].ToString().Trim()) ? "_" : dsDatos.Tables[0].Rows[0]["Telefono"].ToString().Trim();
                this.lblFechaNacCliente.Text    =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["FechaNacimiento"].ToString()) ? "_" :
                                                        String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(dsDatos.Tables[0].Rows[0]["FechaNacimiento"].ToString()));

                this.lblCPCliente.Text          =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["CodigoPostal"].ToString().Trim()) ? "_" : dsDatos.Tables[0].Rows[0]["CodigoPostal"].ToString().Trim();
                this.lblColonia.Text            =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["Colonia"].ToString().Trim()) ? "_" : dsDatos.Tables[0].Rows[0]["Colonia"].ToString().Trim();
                this.lblEstadoCliente.Text      =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["Estado"].ToString().Trim()) ? "_" : dsDatos.Tables[0].Rows[0]["Estado"].ToString().Trim();

                this.lblFechaRegCliente.Text    =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["FechaCreacion"].ToString()) ? "_" :
                                                        String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(dsDatos.Tables[0].Rows[0]["FechaCreacion"].ToString()));

                RegistraEnBitacora(int.Parse(this.txtID_Cliente.Text), (int)AccionBitacora.Datos);
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
        /// Consulta en base de datos los tags del cliente para llenar el combo del FieldSet
        /// de Movimientos.
        /// </summary>
        protected void LlenaFieldSetBuscarMov()
        {
            try
            {
                DataSet dsTags = DAOConsultaClientesTeleVIP.ObtieneTagsCliente(
                                    int.Parse(this.txtID_Cliente.Text), this.Usuario,
                                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                this.StoreTagsPorCuenta.DataSource = dsTags;
                this.StoreTagsPorCuenta.DataBind();

                if (dsTags.Tables[0].Rows.Count == 1)
                {
                    this.cBoxTags.SelectedIndex = 0;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Movimientos", err.Mensaje()).Show();
            }
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
                DataSet dsResumen = LNClientesTeleVIP.ConsultaResumenMeses(
                    this.cBoxTags.SelectedItem.Value,
                    this.txtCuenta.Text, this.Usuario);

                if (dsResumen.Tables[0].Rows.Count > 0)
                {
                    this.lblMes1.Text = dsResumen.Tables[0].Rows[0]["NombreMes"].ToString().Trim();
                    this.lblAcumMes1.Text = String.Format("{0:f2}", float.Parse(dsResumen.Tables[0].Rows[0]["MontoAcumulado"].ToString().Trim()));

                    if (dsResumen.Tables[0].Rows.Count > 1)
                    {
                        this.lblMes2.Text = dsResumen.Tables[0].Rows[1]["NombreMes"].ToString().Trim();
                        this.lblAcumMes2.Text = String.Format("{0:f2}", float.Parse(dsResumen.Tables[0].Rows[1]["MontoAcumulado"].ToString().Trim()));

                        if (dsResumen.Tables[0].Rows.Count > 2)
                        {
                            this.lblMes3.Text = dsResumen.Tables[0].Rows[2]["NombreMes"].ToString().Trim();
                            this.lblAcumMes3.Text = String.Format("{0:f2}", float.Parse(dsResumen.Tables[0].Rows[2]["MontoAcumulado"].ToString().Trim()));
                        }
                    }

                    this.lblAcumuladoPeriodo.Text = String.Format("{0:f2}",
                        (float.Parse(lblAcumMes1.Text) + float.Parse(lblAcumMes2.Text) + float.Parse(lblAcumMes3.Text)));
                }


                DataSet dsMovimientos = new DataSet();

                dsMovimientos = LNClientesTeleVIP.ConsultaMovimientos(
                    Convert.ToDateTime(this.dfFechaInicialMov.Text),
                    Convert.ToDateTime(this.dfFechaFinalMov.Text),
                    this.cBoxTags.SelectedItem.Value,
                    this.txtCuenta.Text, this.Usuario);

                if (dsMovimientos.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Movimientos", "No Existen Movimientos para el Tag en el Periodo Solicitado").Show();
                }
                else
                {
                    for (int registro = 0; registro < dsMovimientos.Tables[0].Rows.Count; registro++)
                    {
                        dsMovimientos.Tables[0].Rows[registro]["EsAplicable"] =
                            Convert.ToBoolean(dsMovimientos.Tables[0].Rows[registro]["EsAplicable"]) ?
                            "Sí" : "No";

                        dsMovimientos.Tables[0].Rows[registro]["SALIDA"] =
                            dsMovimientos.Tables[0].Rows[registro]["ENTRADA"].ToString().Trim() + " - " +
                            dsMovimientos.Tables[0].Rows[registro]["SALIDA"].ToString().Trim();

                        dsMovimientos.Tables[0].Rows[registro]["TARIFA"] = String.Format("{0:f2}",
                            float.Parse(dsMovimientos.Tables[0].Rows[registro]["TARIFA"].ToString().Trim()));
                    }

                    GridResultadosMov.GetStore().DataSource = dsMovimientos;
                    GridResultadosMov.GetStore().DataBind();

                    btnExportExcel.Disabled = false;
                }

                RegistraEnBitacora(int.Parse(this.txtID_Cliente.Text), (int)AccionBitacora.Movimientos);
            }

            catch (CAppException err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Movimientos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Movimientos", "Ocurrió un Error al Buscar los Movimientos").Show();
            }
        }

        /// <summary>
        /// Controla el evento SUBMIT al querer exportar los resultados de movimientos en Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        //protected void StoreResultadosMov_Submit(object sender, StoreSubmitDataEventArgs e)
        //{
        //    string format = this.FormatType.Value.ToString();

        //    XmlNode xml = e.Xml;

        //    this.Response.Clear();

        //    switch (format)
        //    {
        //        case "xls":
        //            this.Response.ContentType = "application/vnd.ms-excel";
        //            this.Response.AddHeader("Content-Disposition", "attachment; filename=Movimientos TAG.xls");
        //            XslCompiledTransform xtExcel = new XslCompiledTransform();
        //            xtExcel.Load(Server.MapPath("xslFiles/Excel.xsl"));
        //            xtExcel.Transform(xml, null, Response.OutputStream);
        //            break;                
        //    }

        //    this.Response.End();

        //}

        /// <summary>
        /// Extrae el grid de movimientos del backend para generarlo y exportarlo a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            string gridResultJson = e.ExtraParams["GridToExport"];
            string reportName = "Movimientos_TAG";

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
                ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.Number);
            }

            return ws;
        }

        /// <summary>
        /// Consulta en base de datos los tags del cliente para llenar el combo del FieldSet
        /// de Recompensas.
        /// </summary>
        protected void LlenaFormPanelRecompensas()
        {
            try
            {
                DataSet dsTags = DAOConsultaClientesTeleVIP.ObtieneTagsCliente(
                                    int.Parse(this.txtID_Cliente.Text), this.Usuario,
                                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                this.StoreTagsRec.DataSource = dsTags;
                this.StoreTagsRec.DataBind();

                if (dsTags.Tables[0].Rows.Count == 1)
                {
                    this.cBoxTagsBuscarRec.SelectedIndex = 0;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Recompensas", err.Mensaje()).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario de Recompensas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarRecompensas_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet dsRecompensas = new DataSet();

                dsRecompensas = LNClientesTeleVIP.ConsultaRecompensas(
                    this.cBoxTagsBuscarRec.SelectedItem.Value,
                    this.txtCuenta.Text, this.Usuario);

                if (dsRecompensas.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Recompensas", "No Existen Recompensas para el Tag").Show();
                }
                else
                {
                    GridResultadosRec.GetStore().DataSource = dsRecompensas;
                    GridResultadosRec.GetStore().DataBind();
                }

                RegistraEnBitacora(int.Parse(this.txtID_Cliente.Text), (int)AccionBitacora.Recompensas);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Recompensas", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Recompensas", "Ocurrió un Error al Buscar las Recompensas").Show();
            }
        }

       
        ///// <summary>
        ///// Consulta en base de datos el cátálogo de actividades para el motivo de la llamada 
        ///// para llenar el combo correspondiente dentro del FieldSetCapturarLlamada
        ///// </summary>
        //protected void LlenaFormPanelLlamada()
        //{
        //    try
        //    {
        //        this.StoreMotivos.DataSource = DAOConsultaClientes.ListaMotivosLlamada(this.Usuario);
        //        this.StoreMotivos.DataBind();

        //        this.FormPanelLlamada.Title = "Nombre: " + this.lblNombreCliente.Text + " " + this.lblApPaternoCliente.Text + " " + this.lblApMaternoCliente.Text;
        //    }

        //    catch (CAppException err)
        //    {
        //        X.Msg.Alert("Capturar Llamada", err.Mensaje()).Show();
        //    }
        //}

        ///// <summary>
        ///// Controla el evento Click al botón Guardar del formulario de captura de llamada,
        ///// invocando la inserción del refistro correspondiente en la bitácora de base de datos
        ///// </summary>
        ///// <param name="sender">Objeto que envía el control</param>
        ///// <param name="e">Argumentos del evento que se ejecutó</param>
        //protected void btnGuardarLlamada_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        LNClientes.RegistraLlamadaCliente(
        //            int.Parse(this.txtID_Cliente.Text),
        //            int.Parse(this.cBoxMotivoLlamada.SelectedItem.Value),
        //            this.txtComentarios.Text, this.Usuario);

        //        X.Msg.Notify("", "La Llamada se Capturó Exitósamente").Show();

        //        RegistraEnBitacora(int.Parse(this.txtID_Cliente.Text), (int)AccionBitacora.Llamada);

        //        FormPanelCapturarLlamada.Reset();
        //    }

        //    catch (CAppException err)
        //    {
        //        X.Msg.Alert("Captura de Llamada", err.Mensaje()).Show();
        //    }

        //    catch (Exception)
        //    {
        //        X.Msg.Alert("Captura de Llamada", "Ocurrió un Error al Registrar la Llamada del Cliente").Show();
        //    }
        //}
    }
}
   