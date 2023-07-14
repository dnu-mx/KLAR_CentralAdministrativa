using ClosedXML.Excel;
using DALAdministracion.BaseDatos;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
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
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace Administracion
{
    public partial class CuentasVIPConsulta : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Cuentas VIP
        private LogHeader LH_ParabConsCtasVIP = new LogHeader();

        #endregion

  

        /// <summary>
        /// Realiza y controla la carga de la página Parámetros Open Pay
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabConsCtasVIP.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabConsCtasVIP.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabConsCtasVIP.User = this.Usuario.ClaveUsuario;
            LH_ParabConsCtasVIP.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabConsCtasVIP);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA CuentasVIP Page_Load()");

                if (!IsPostBack)
                {
                    HttpContext.Current.Session.Add("EsAutorizador", false);
                    HttpContext.Current.Session.Add("EsEjecutor", false);

                    EstableceSubEmisores();

                    HttpContext.Current.Session.Add("ElParametro", null);
                }

                log.Info("TERMINA CuentasVIP Page_Load()");
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
        /// Establece los valores del combo SubEmisor con la información de base de datos
        /// </summary>
        protected void EstableceSubEmisores()
        {
            LogPCI logPCI = new LogPCI(LH_ParabConsCtasVIP);

            try
            {
                logPCI.Info("INICIA ListaColectivasSubEmisor()");
                DataTable dtSubEmisores = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabConsCtasVIP);
                logPCI.Info("TERMINA ListaColectivasSubEmisor()");

                List<ColectivaComboPredictivo> ComboList = new List<ColectivaComboPredictivo>();

                foreach (DataRow drCol in dtSubEmisores.Rows)
                {
                    var colectivaCombo = new ColectivaComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt64(drCol["ID_Colectiva"].ToString()),
                        ClaveColectiva = drCol["ClaveColectiva"].ToString(),
                        NombreORazonSocial = drCol["NombreORazonSocial"].ToString()
                    };
                    ComboList.Add(colectivaCombo);
                }

                this.StoreSubEmisores.DataSource = ComboList;
                this.StoreSubEmisores.DataBind();
            }
            catch (CAppException caEx)
            {
                logPCI.Error(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo SubEmisor
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceProductosAut(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabConsCtasVIP);

            try
            {
                this.StoreProductosAut.RemoveAll();

                log.Info("INICIA ObtieneProductosDeColectiva()");
                this.StoreProductosAut.DataSource = DAOProducto.ObtieneProductosDeColectiva(
                    Convert.ToInt32(this.cBoxClienteAut.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabConsCtasVIP);
                log.Info("TERMINA ObtieneProductosDeColectiva()");
                this.StoreProductosAut.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Productos", "Error al obtener los Productos del SubEmisor").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel de Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarTajetasVIP_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabConsCtasVIP);

            try
            {
                pCI.Info("INICIA ConsultaSolicitudesCambioCuentaVIP()");
                this.StoreParamsTarjetasVIPDetalle.DataSource = LNCuentas.ConsultaTarjetasVIPDetalle(
                    Convert.ToInt64(this.cBoxProductoAut.SelectedItem.Value),
                    this.txtTarjetaAut.Text, LH_ParabConsCtasVIP);
                pCI.Info("TERMINA ConsultaSolicitudesCambioCuentaVIP()");

                this.StoreParamsTarjetasVIPDetalle.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Consulta", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Consulta", "Ocurrió un error al obtener las Cuentas VIP").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del panel de Solicitudes de Cambio, restableciendo los controles
        /// de dicho panel a su estado de carga inicial
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarAut_Click(object sender, EventArgs e)
        {
            this.cBoxClienteAut.Reset();

            this.cBoxProductoAut.Reset();
            this.StoreProductosAut.RemoveAll();
            this.txtTarjetaAut.Reset();

            this.StoreParamsTarjetasVIPDetalle.RemoveAll();
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
                string reportName = "Consulta";

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

            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_ParabConsCtasVIP);
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
                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 9).SetDataType(XLCellValues.DateTime);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finalizar la máscara al botón de
        /// exportar reporte a excel
        /// </summary>
        [DirectMethod(Namespace = "ConsultaCtasVIP")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}
