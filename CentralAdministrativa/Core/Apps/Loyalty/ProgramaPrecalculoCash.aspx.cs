using ClosedXML.Excel;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.IO;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;

namespace CentroContacto
{
    public partial class ProgramaPrecalculoCash : DALCentralAplicaciones.PaginaBaseCAPP
    {
        const int ESTATUS_ELIMINAR = 4;

        /// <summary>
        /// Realiza y controla la carga de la página Cambio de Nivel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    var fecha = DateTime.Now;

                    HttpContext.Current.Session.Add("DtPrecalculo", null);
                    dtFechaCalculo.MinDate = fecha.Date.AddDays(1);
                    //LlenaComboNivelesLealtad();
                    LlenarGridResultados();
                }
            }

            catch (Exception ex)
            {
                DALCentralAplicaciones.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Llena grid 
        /// </summary>
        protected void LlenarGridResultados()
        {
            try
            {
                DataSet dsResultados = DAOPrecalculoPuntosCash.ObtienePrecalculoPuntos(this.Usuario);

                GridPrecalculo.GetStore().DataSource = dsResultados;
                GridPrecalculo.GetStore().DataBind();


                
                //DataTable dtMovimientos = new DataTable();
                //dtMovimientos = HttpContext.Current.Session["DtPrecalculo"] as DataTable;
                //dtMovimientos = DAOPrecalculoPuntosCash.ObtieneReportePrecalculo(this.Usuario);
                //HttpContext.Current.Session.Add("DtPrecalculo", dtMovimientos);
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Cambio de Nivel", "Ocurrió un error al consultar los Niveles.").Show();
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Controla el evento Refresh en el grid de resultados, invocando nuevamente
        /// la búsqueda de clientes a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento Refresh Data del Store Clientes</param>
        protected void StorePrecalculo_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            LlenarGridResultados();
        }


        [DirectMethod]
        public void Descargar_Event()
        {
            btnDownloadHide.FireEvent("click");
            //X.Msg.Alert("Descargar ID: " + idProgramacion, "Descargando...").Show();
            //ExportToExcel();
        }

        protected void Download(object sender, DirectEventArgs e)
        {
            ExportToExcel();
        }


        [DirectMethod]
        public void Eliminar_Event(int idProgramacion)
        {
            //X.Msg.Alert("Eliminar ID: " + idProgramacion, "Eliminado...").Show();

            try
            {
                LNCash.ActualizaPrecalculo(idProgramacion, ESTATUS_ELIMINAR, this.Usuario);
                X.Msg.Notify("Programación de Precalculo", "Programación Eliminada <br /><br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                LlenarGridResultados();
            }

            catch (Exception err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Notify("Programación de Precalculo", err.Message.ToString()).Show();
            }
        }


        /// <summary>
        /// Controla el evento Click al botón de Guardar en PropertyGrid con los
        /// valores de los parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnAgregar_Click(object sender, DirectEventArgs e)
        {
            try
            {
                LNCash.InsertaPrecalculo(Convert.ToDateTime(this.dtFechaCalculo.Text), this.Usuario);
                X.Msg.Notify("Programación de Precalculo", "Nueva Programación Agregada <br /><br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                LlenarGridResultados();
            }

            catch (Exception err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Notify("Programación de Precalculo", err.Message.ToString()).Show();
            }
        }

        /// <summary>
        /// Exporta el reporte generado a un archivo Excel 
        /// </summary>
        //public void ExportToExcel()
        //{
        //    HttpResponse response = HttpContext.Current.Response;

        //    try
        //    {
        //        DataSet dataSet = new DataSet();
        //        DataTable table = new DataTable();

        //        //dataSet.Tables.Add(table);
        //        dataSet = DAOPrecalculoPuntosCash.ObtieneReportePrecalculo(this.Usuario);

        //        response.Clear();
        //        response.Charset = "";
        //        //response.Flush();

        //        response.ContentType = "application/vnd.ms-excel";
        //        response.AddHeader("Content-Disposition", "attachment;filename=\"ExcelFile.xls\"");

        //        using (StringWriter stringWriter = new StringWriter())
        //        using (HtmlTextWriter htmlTextWriter = new HtmlTextWriter(stringWriter))
        //        {
        //            DataGrid dataGrid = new DataGrid { DataSource = dataSet.Tables[0] };

        //            dataGrid.DataBind();
        //            dataGrid.RenderControl(htmlTextWriter);

        //            response.Write(stringWriter.ToString());

        //        }
        //        //response.End();
        //        //HttpContext.Current.ApplicationInstance.CompleteRequest();
        //    }

        //    catch (ThreadAbortException ex)
        //    {
        //        DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
        //        X.Msg.Alert("Beneficios", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
        //    }

        //    response.End();
        //}

        /// <summary>
        /// Exporta el reporte generado a un archivo Excel 
        /// </summary>
        public void ExportToExcel()
        {
            try
            {
                DataTable _dtMovimientos = new DataTable();
                _dtMovimientos = HttpContext.Current.Session["DtPrecalculo"] as DataTable;
                _dtMovimientos = DAOPrecalculoPuntosCash.ObtieneReportePrecalculo(this.Usuario);
                HttpContext.Current.Session.Add("DtPrecalculo", _dtMovimientos);

                string reportName = "ReportePrecalculo";
                //DataTable _dtMovimientos = HttpContext.Current.Session["DtPrecalculo"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtMovimientos, reportName);

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

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Beneficios", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
            }
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "ReporteMask")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

    }
}