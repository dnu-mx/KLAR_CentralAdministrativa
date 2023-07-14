using ClosedXML.Excel;
using DALAdministracion.BaseDatos;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using DALPuntoVentaWeb.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;

namespace TpvWeb
{
    public partial class DiasNoBancarios : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Dias No Bancarios
        private LogHeader LH_DiasNoBancarios = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Días No Bancarios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_DiasNoBancarios.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_DiasNoBancarios.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_DiasNoBancarios.User = this.Usuario.ClaveUsuario;
            LH_DiasNoBancarios.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_DiasNoBancarios);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA DiasNoBancarios Page_Load()");

                int index = 0;
                this.CalendarPanel1.EventStore.AddStandardFields();
                this.CalendarPanel1.EventStore.Reader[0].Fields.Add(new RecordField("Fecha"));

                if (!X.IsAjaxRequest)
                {
                    hdnMeses.Value = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "MesesDiasNoBancarios").Valor);
                    int totalMeses = Convert.ToInt16(hdnMeses.Value);

                    DatePicker1.MaxDate = DateTime.Today.AddMonths(totalMeses);
                    DatePicker1.MinDate = DateTime.Today.AddMonths(-totalMeses);

                    log.Info("INICIA ListaCatalogoPaises()");
                    DataTable dtPaises = DAOTarjetaCuenta.ListaCatalogoPaises(LH_DiasNoBancarios);
                    log.Info("TERMINA ListaCatalogoPaises()");

                    foreach (DataRow dr in dtPaises.Rows)
                    {
                        if (dr["Clave"].ToString() == "484") //México
                        {
                            cBoxPais.SelectedIndex = index;
                            cBoxPais.SetValue(dr["ID_Pais"]);
                            break;
                        }
                        index++;
                    }

                    StorePaises.DataSource = dtPaises;
                    StorePaises.DataBind();

                    EstablecePermisoEscritura();

                    EstableceDiasInhabiles();
                }

                log.Info("TERMINA DiasNoBancarios Page_Load()");
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
        /// Establece en control oculto si el rol del usuario tiene permisos de 
        /// escritura en el calendario de días no bancarios
        /// </summary>
        protected void EstablecePermisoEscritura()
        {
            hdnSoloConsulta.Value = 1;
            this.Usuario.Roles.Sort();

            foreach (string rol in this.Usuario.Roles)
            {
                if (rol.ToUpper().Contains("ADMIN"))
                {
                    hdnSoloConsulta.Value = 0;
                    break;
                }
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo País, llamando al método
        /// que establece los días inhábiles
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void SeleccionaPais(object sender, EventArgs e)
        {
            EstableceDiasInhabiles();
        }

        /// <summary>
        /// Establece en el store del control CalendarPanel los eventos (días inhábiles)
        /// </summary>
        protected void EstableceDiasInhabiles()
        {
            LogPCI pCI = new LogPCI(LH_DiasNoBancarios);
            pCI.Info("EstableceDiasInhabiles()");

            try
            {
                int numEvento = 0;
                DateTime laFecha = new DateTime();
                Store store = this.CalendarPanel1.EventStore;

                DataTable dtDias = ConsultaDiasInhabiles();
                var losDias = new List<DiaNoBancario>();

                if (dtDias.Rows.Count < 1)
                {
                    X.Msg.Alert("Calendario Días No Bancarios", "No existen coincidencias con el país y/o periodo.").Show();
                }
                else
                {
                    foreach (DataRow dia in dtDias.Rows)
                    {
                        laFecha = Convert.ToDateTime(dia["Fecha"]);
                        numEvento += 1;

                        DiaNoBancario unDia = new DiaNoBancario();

                        unDia.Title = "DIA NO BANCARIO";
                        unDia.CalendarId = 4;
                        unDia.StartDate = laFecha;
                        unDia.EndDate = laFecha;
                        unDia.IsAllDay = true;
                        unDia.EventId = numEvento;

                        losDias.Add(unDia);
                    }

                    btnListaAnual.Disabled = false;
                    btnExportExcel.Disabled = false;
                }

                store.DataSource = losDias;
                store.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Calendario Días No Bancarios", caEx.Mensaje()).Show();
                throw caEx;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Calendario Días No Bancarios", "Error al establecer los Días no Bancarios").Show();
            }
        }

        /// <summary>
        /// Controla la llamada a la consulta de días inhábiles en base de datos,
        /// estableciendo los valores obtenidos en un objeto DataTable
        /// </summary>
        /// <returns>DataTable con los registros de la consulta</returns>
        protected DataTable ConsultaDiasInhabiles()
        {
            DataTable dt = new DataTable();
            LogPCI log = new LogPCI(LH_DiasNoBancarios);
            log.Info("ConsultaDiasInhabiles()");

            try
            {
                int meses = Convert.ToInt32(hdnMeses.Value);

                DateTime FechaInicial = DateTime.Today.AddMonths(-meses);
                DateTime FechaFinal = DateTime.Today.AddMonths(meses);

                log.Info("INICIA ListaDiasInhabiles()");
                dt = DAOAdministrarBanca.ListaDiasInhabiles(FechaInicial, FechaFinal,
                    int.Parse(cBoxPais.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_DiasNoBancarios);
                log.Info("TERMINA ListaDiasInhabiles()");
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }

            return dt;
        }

        /// <summary>
        /// Controla la creación de un nuevo día no bancario tanto en base de datos
        /// como en el CalendarPanel correspondiente
        /// </summary>
        /// <param name="fecha">Fecha seleccionada del CalendarPanel</param>
        [DirectMethod(Namespace = "DNB")]
        public void NuevoDiaNoBancario(object fecha)
        {
            LogPCI unLog = new LogPCI(LH_DiasNoBancarios);

            if (String.IsNullOrEmpty(cBoxPais.SelectedItem.Value))
            {
                X.Msg.Alert("Calendario Días No Bancarios", "Selecciona un país").Show();
                return;
            }

            try
            {
                DateTime laFecha = Convert.ToDateTime(fecha);

                unLog.Info("INICIA CreaCorteDiaInhabil()");
                LNAdministrarBanca.CreaCorteDiaInhabil(laFecha, int.Parse(cBoxPais.SelectedItem.Value), this.Usuario, LH_DiasNoBancarios);
                unLog.Info("TERMINA CreaCorteDiaInhabil()");

                X.Msg.Notify("", "Fecha creada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                EstableceDiasInhabiles();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Calendario Días No Bancarios", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Calendario Días No Bancarios", "Ocurrió un error al establecer la fecha como Día no Bancario").Show();
            }
        }

        /// <summary>
        /// Controla la eliminación de un día no bancario tanto en base de datos
        /// como en el CalendarPanel correspondiente
        /// </summary>
        /// <param name="Fecha">Fecha por eliminar</param>
        [DirectMethod(Namespace = "DNB")]
        public void EliminaDiaNoBancario(DateTime Fecha)
        {
            LogPCI logPCI = new LogPCI(LH_DiasNoBancarios);

            try
            {
                DateTime laFecha = Convert.ToDateTime(Fecha);

                logPCI.Info("INICIA BorraCorteDiaInhabil()");
                LNAdministrarBanca.BorraCorteDiaInhabil(Fecha, int.Parse(cBoxPais.SelectedItem.Value), this.Usuario, LH_DiasNoBancarios);
                logPCI.Info("TERMINA BorraCorteDiaInhabil()");
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Calendario Días No Bancarios", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Calendario Días No Bancarios", "Ocurrió un error al eliminar la fecha como Día no Bancario").Show();
            }
        }

        /// <summary>
        /// Exporta el listado de días inhábiles a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportarExcel(object sender, DirectEventArgs e)
        {
            try
            {
                string reportName = "CalendarioDiasNoBancarios";
                DataTable _dtCalendario = ConsultaDiasInhabiles();

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtCalendario, reportName);

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
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Calendario Días No Bancarios", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_DiasNoBancarios);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Calendario Días No Bancarios", "Ocurrió un Error al Exportar a Excel").Show();
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
                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.DateTime);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Controla el evento Click del botón Ver Lista Anual, solicitando los registros
        /// a base de datos y estableciéndolos en el grid correspondiente
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void VerListaAnual(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_DiasNoBancarios);

            try
            {
                int meses = Convert.ToInt32(hdnMeses.Value);

                DateTime FechaInicial = DateTime.Today.AddMonths(-meses);
                DateTime FechaFinal = DateTime.Today.AddMonths(meses);

                pCI.Info("INICIA ListaCalendarioAnual()");
                DataTable dtListaAnual = DAOAdministrarBanca.ListaCalendarioAnual(FechaInicial, FechaFinal,
                   int.Parse(cBoxPais.SelectedItem.Value), this.Usuario,
                   Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                   LH_DiasNoBancarios);
                pCI.Info("INICIA ListaCalendarioAnual()");

                if (dtListaAnual.Rows.Count < 1)
                {
                    X.Msg.Alert("Lista Anual", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else
                {
                    StoreCalendario.DataSource = dtListaAnual;
                    StoreCalendario.DataBind();

                    btnListaAnual.Disabled = false;
                    btnExportExcel.Disabled = false;

                    WdwCalendarioLista.Show();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Lista Anual", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Lista Anual", "Ocurrió un error al obtener la lista anual de Días no Bancarios").Show();
            }
        }
    }
}