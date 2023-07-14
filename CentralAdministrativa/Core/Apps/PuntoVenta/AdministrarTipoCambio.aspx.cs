using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;


namespace TpvWeb
{
    public partial class AdministrarTipoCambio : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Administrar Tipo de Cambio
        private LogHeader LH_AdminTipoCambio = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Administrar Tipo de Cambio
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_AdminTipoCambio.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_AdminTipoCambio.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_AdminTipoCambio.User = this.Usuario.ClaveUsuario;
            LH_AdminTipoCambio.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_AdminTipoCambio);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdministrarTipoCambio Page_Load()");

                if (!IsPostBack)
                {
                    log.Info("INICIA ListaDivisas()");
                    DataSet dsDivisas = DAOAdministrarBanca.ListaDivisas(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_AdminTipoCambio);
                    log.Info("INICIA ListaDivisas()");

                    this.StoreMoneda.DataSource = dsDivisas;
                    this.StoreMoneda.DataBind();

                    foreach (DataRow row in dsDivisas.Tables[0].Rows)
                    {
                        if (row["ClaveMoneda"].ToString().Trim() == "USD")
                        {
                            hdnIdUSD.Text = row["ID_Divisa"].ToString();

                            //Se prestablece como default en el combo la divisa USD
                            cBoxMoneda.SetValue(Convert.ToInt32(row["ID_Divisa"].ToString()));
                        }
                    }
                    
                    dfFecha.MaxDate = DateTime.Today;
                    dfFecha.SetValue(DateTime.Today);

                    this.txtTipoCambio.Focus();

                    LlenaGridTiposCambio();
                }

                log.Info("TERMINA AdministrarTipoCambio Page_Load()");
            }
            catch (CAppException)
            {
                errRedirect = "../ErrorInicializarPagina.aspx";
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

        /// <summary>
        /// Controla el evento Click al botón Agregar, llamando a la creación
        /// de un nuevo tipo de cambio en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_AdminTipoCambio);

            try
            {
                string resp = "";
                
                logPCI.Info("INICIA CreaNuevoTipoCambio()");
                resp =  LNAdministrarBanca.CreaNuevoTipoCambio(dfFecha.SelectedDate.ToString(),
                            int.Parse(cBoxMoneda.SelectedItem.Value.ToString()),
                            this.txtTipoCambio.Text, this.Usuario, LH_AdminTipoCambio);
                logPCI.Info("TERMINA CreaNuevoTipoCambio()");

                if (String.IsNullOrEmpty(resp))
                {
                    X.Msg.Notify("", "Tipo de Cambio Añadido" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                    LlenaGridTiposCambio();
                }
                else
                {
                    X.Msg.Alert("Tipo de Cambio", resp).Show();
                }
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Tipo de Cambio", caEx.Mensaje() + " Cod.(" + caEx.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Tipo de Cambio", "Ocurrió un error al añadir el Tipo de Cambio").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar, restableciendo
        /// los controles de la página al valor de carga origen
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            FormPanelTipoCambio.Reset();

            dfFecha.SetValue(DateTime.Today);
            cBoxMoneda.SetValue(Convert.ToInt32(hdnIdUSD.Text));
            this.txtTipoCambio.Reset();
            this.txtTipoCambio.Focus();

            StoreTiposCambio.RemoveAll();
            LlenaGridTiposCambio();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SeleccionaDivisa(object sender, EventArgs e)
        {
            dfFecha.SetValue(DateTime.Today);
            this.txtTipoCambio.Reset();
            this.txtTipoCambio.Focus();

            StoreTiposCambio.RemoveAll();
            LlenaGridTiposCambio();
        }

        /// <summary>
        /// Llena el grid de tipos de cambio con la información de base de datos
        /// </summary>
        protected void LlenaGridTiposCambio()
        {
            LogPCI unLog = new LogPCI(LH_AdminTipoCambio);

            try
            {
                btnExportExcel.Disabled = true;

                unLog.Info("INICIA ListaTiposCambioDivisa()");
                DataSet dsTiposDeCambio = DAOAdministrarBanca.ListaTiposCambioDivisa(
                    int.Parse(cBoxMoneda.SelectedItem.Value.ToString()), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    LH_AdminTipoCambio);
                unLog.Info("TERMINA ListaTiposCambioDivisa()");

                if (dsTiposDeCambio.Tables[0].Rows.Count > 0)
                {
                    btnExportExcel.Disabled = false;
                }

                this.StoreTiposCambio.DataSource = dsTiposDeCambio;
                this.StoreTiposCambio.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Tipos de Cambio", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Tipos de Cambio", "Ocurrió un error al obtener los Tipos de Cambio").Show();
            }
        }

        /// <summary>
        /// Exporta el grid de resultados al backend, para generar y exportarlo a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            try
            {
                string gridResultJson = e.ExtraParams["GridToExport"];
                string reportName = "TiposDeCambio";

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
                this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte_" + reportName + ".xlsx");

                //Se envía el reporte como respuesta
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(this.Response.OutputStream);
                    memoryStream.Close();
                }
            }

            catch(Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_AdminTipoCambio);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Tipos de Cambio", "Ocurrió un error al exportar el reporte a Excel").Show();
            }

            finally
            {
                if (this.Response != null)
                {
                    this.Response.Clear();
                    this.Response.ClearContent();
                    this.Response.OutputStream.Dispose();

                    this.Response.Flush();
                    this.Response.Close();

                    GC.Collect();
                }
            }
        }

        /// <summary>
        /// Establece el formato deseado a las columnas de la hoja de trabajo por exportar
        /// </summary>
        /// <param name="ws">Hoja de trabajo</param>
        /// <param name="rowsNum">Total de filas de la hoja de trabajo</param>
        /// <returns></returns>
        protected IXLWorksheet FormatWsColumns(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                ws.Column(1).Hide();
                ws.Column(2).Hide();
                ws.Column(6).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.Number);
                }

                return ws;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de cuentas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_AdminTipoCambio);

            try
            {
                String comando = (String)e.ExtraParams["Comando"];
                char[] charsToTrim = { '*', '"', ' ', 'T' };

                hdnIdTipoCambio.Text = e.ExtraParams["ID_TipoCambioDivisa"].ToString();
                string Fecha = String.Format("{0:dd-MM-yyyy}", 
                    Convert.ToDateTime(e.ExtraParams["Fecha"].ToString().Trim(charsToTrim)));
                string Moneda = e.ExtraParams["Moneda"].ToString().Trim(charsToTrim);
                string Pesos = e.ExtraParams["Pesos"].ToString();

                switch (comando)
                {
                    case "Edit":
                        FormPanelEditar.Reset();

                        txtFecha.Text = Fecha;
                        txtMoneda.Text = Moneda;
                        txtTipoCambioEditar.Text = Pesos;

                        wdwEditar.Show();
                        break;

                    case "Delete":
                        unLog.Info("INICIA BorraTipoCambio()");
                        LNAdministrarBanca.BorraTipoCambio(int.Parse(hdnIdTipoCambio.Text), this.Usuario, LH_AdminTipoCambio);
                        unLog.Info("INICIA BorraTipoCambio()");

                        X.Msg.Notify("", "Tipo de Cambio Borrado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                        wdwEditar.Hide();

                        LlenaGridTiposCambio();
                        break;

                    default: break;
                }
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Tipo de Cambio", caEx.Mensaje() + " Cod.(" + caEx.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Tipo de Cambio", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar Cambios de la ventana
        /// Editar Tipo de Cambio
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_AdminTipoCambio);

            try
            {
                unLog.Info("INICIA ModificaTipoCambio()");
                LNAdministrarBanca.ModificaTipoCambio(int.Parse(hdnIdTipoCambio.Text), this.txtTipoCambioEditar.Text,
                    this.Usuario, LH_AdminTipoCambio);
                unLog.Info("INICIA ModificaTipoCambio()");

                X.Msg.Notify("", "Tipo de Cambio Modificado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                LlenaGridTiposCambio();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Editar Tipo de Cambio", caEx.Mensaje() + " Cod.(" + caEx.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Editar Tipo de Cambio", "Ocurrió un error al editar el tipo de cambio").Show();
            }
            finally
            {
                this.wdwEditar.Hide();
            }
        }
    }
}