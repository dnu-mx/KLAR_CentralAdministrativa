using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALPuntoVentaWeb.BaseDatos;
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

namespace TpvWeb
{
    public partial class Reporte_ConciliacionOpenPay : DALCentralAplicaciones.PaginaBaseCAPP
    {
        private LogHeader LH_Reporte = new LogHeader();

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_Reporte.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_Reporte.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_Reporte.User = this.Usuario.ClaveUsuario;
            LH_Reporte.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_Reporte);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA Reporte_ConciliacionOpenPay Page_Load()");

                if (!IsPostBack)
                {
                    log.Info("INICIA ListaColectivasSubEmisor()");
                    DataTable dtColectivas =
                        DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_Reporte);
                    log.Info("TERMINA ListaColectivasSubEmisor()");

                    List<ColectivaComboPredictivo> ComboList = new List<ColectivaComboPredictivo>();

                    foreach (DataRow drCol in dtColectivas.Rows)
                    {
                        var colectivaCombo = new ColectivaComboPredictivo()
                        {
                            ID_Colectiva = Convert.ToInt32(drCol["ID_Colectiva"]),
                            ClaveColectiva = drCol["ClaveColectiva"].ToString(),
                            NombreORazonSocial = drCol["NombreORazonSocial"].ToString()
                        };
                        ComboList.Add(colectivaCombo);
                    }
                   
                    this.StoreSubEmisores.DataSource = ComboList;
                    this.StoreSubEmisores.DataBind();
                }

                log.Info("TERMINA Reporte_ConciliacionOpenPay Page_Load()");
            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
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

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_Reporte);
            try
            {                
                string idColectiva = cBoxCliente.SelectedItem.Value;
                DateTime fecheaInicio = dfFechaInicio.SelectedDate;
                DateTime fechaFin = dfFechaFin.SelectedDate;
                log.Info("INICIA ListaRegistrosOpenPay()");
                DataSet ds = DAOReportes.ListaRegistrosOpenPay(idColectiva, fecheaInicio, fechaFin, LH_Reporte);
                log.Info("TERMINA ListaRegistrosOpenPay()");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    btnExportExcel.Disabled = false;
                    this.StoreOperaciones.DataSource = ds;
                    this.StoreOperaciones.DataBind();
                }
                else
                {
                    this.StoreOperaciones.DataSource = "";
                    this.StoreOperaciones.DataBind();
                    btnExportExcel.Disabled = true;
                    X.Msg.Alert("Reporte-OpenPay", "No existen coincidencias con la búsqueda solicitada").Show();
                }

                HttpContext.Current.Session.Add("dsResultados", ds);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Reporte-OpenPay", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
                X.Msg.Alert("Reporte-OpenPay", "Ocurrió un error al obtener las operaciones OpenPay").Show();
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            cBoxCliente.SelectedItem.Value = "";
            dfFechaInicio.Value = "";
            dfFechaFin.Value = "";
        }

        [DirectMethod(Namespace = "RepOpenPay")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        protected void DownloadOpenPay(object sender, DirectEventArgs e)
        {
            ExportDTDetalleToExcel();
        }

        [DirectMethod(Namespace = "RepOpenPay")]
        public void ExportDTDetalleToExcel()
        {
            LogPCI unLog = new LogPCI(LH_Reporte);
            try
            {
                string reportName = "ReporteOpenPay";
                DataSet _dsResultados = HttpContext.Current.Session["dsResultados"] as DataSet;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dsResultados.Tables[0], reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWSComercioTarjeta(ws, ws.Column(1).CellsUsed().Count());

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

            catch (Exception ex)
            {
                unLog.Error("Error al exportar el reporte a Excel");
                unLog.ErrorException(ex); ;
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

        protected IXLWorksheet FormatWSComercioTarjeta(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                //ws.Column(1).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 17).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 18).SetDataType(XLCellValues.DateTime);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}