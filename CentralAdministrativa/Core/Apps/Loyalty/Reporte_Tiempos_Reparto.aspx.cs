﻿using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace CentroContacto
{
    public partial class Reporte_Tiempos_Reparto : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    LlenaSucursales();
                    LLenaTiposServicio();
                    LLenaRepartidores();

                    //Se prestablecen las fechas de consulta del reporte
                    datInicio.SetValue(DateTime.Today);
                    datInicio.MaxDate = DateTime.Today;

                    datFinal.SetValue(DateTime.Today);
                    datFinal.MaxDate = DateTime.Today;

                    PagingToolBar1.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtTiempos", null);
                }
            }

            catch (Exception err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de tiempos de reparto, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelTiempos(int RegistroInicial, string Columna, SortDirection Orden)
        {
            DateTime fechaInicial, fechaFinal;

            fechaInicial = Convert.ToDateTime(datInicio.SelectedDate);
            fechaFinal = Convert.ToDateTime(datFinal.SelectedDate);

            //Se validan fechas
            if (DateTime.Compare(fechaInicial, fechaFinal) > 0)
            {
                throw new CAppException(8006, "La fecha inicial debe ser menor o igual a la fecha final");
            }

            btnExportXML.Disabled = true;
            btnExportExcel.Disabled = true;
            btnExportCSV.Disabled = true;

            try
            {
                DataTable dtTiempos = new DataTable();

                dtTiempos = HttpContext.Current.Session["DtTiempos"] as DataTable;

                if (dtTiempos == null)
                {
                    dtTiempos = LNReportes.ReporteTiemposRepartoMoshi(fechaInicial, fechaFinal,
                        String.IsNullOrEmpty(cmbSucursal.SelectedItem.Value) ? 0 : int.Parse(cmbSucursal.SelectedItem.Value),
                        String.IsNullOrEmpty(cmbTipoServicio.SelectedItem.Value) ? 0 : int.Parse(cmbTipoServicio.SelectedItem.Value),
                        String.IsNullOrEmpty(cmbRepartidor.SelectedItem.Value) ? 0 : int.Parse(cmbRepartidor.SelectedItem.Value),
                        this.Usuario);

                    HttpContext.Current.Session.Add("DtTiempos", dtTiempos);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtTiempos == null)
                {
                    X.Msg.Alert("Tiempos de Reparto", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtTiempos.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Tiempos de Reparto", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepTiemposRepartoMoshi.ClicDePaso()",
                                Text = "Aceptar"
                            },
                            No = new MessageBoxButtonConfig
                            {
                                Text = "Cancelar"
                            }
                        }).Show();
                }

                else
                {
                    int TotalRegistros = dtTiempos.Rows.Count;

                    (this.StoreTiempos.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtTiempos.Clone();
                    DataTable dtToGrid = dtTiempos.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtTiempos.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingToolBar1.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtTiempos.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreTiempos.DataSource = dtToGrid;
                    StoreTiempos.DataBind();

                    btnExportXML.Disabled = false;
                    btnExportExcel.Disabled = false;
                    btnExportCSV.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tiempos de Reparto", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Tiempos de Reparto", "Ocurrió un Error al Consultar el Reporte").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        protected void StoreSubmit(object sender, StoreSubmitDataEventArgs e)
        {
            string format = this.FormatType.Value.ToString();

            XmlNode xml = e.Xml;

            this.Response.Clear();

            switch (format)
            {
                case "xml":
                    string strXml = xml.OuterXml;
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.xml");
                    this.Response.AddHeader("Content-Length", strXml.Length.ToString());
                    this.Response.ContentType = "application/xml";
                    this.Response.Write(strXml);
                    break;

                case "xls":
                    this.Response.ContentType = "application/vnd.ms-excel";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.xls");
                    XslCompiledTransform xtExcel = new XslCompiledTransform();
                    xtExcel.Load(Server.MapPath("xslFiles/Excel.xsl"));
                    xtExcel.Transform(xml, null, Response.OutputStream);
                    break;

                case "csv":
                    this.Response.ContentType = "application/octet-stream";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.csv");
                    XslCompiledTransform xtCsv = new XslCompiledTransform();
                    xtCsv.Load(Server.MapPath("xslFiles/Csv.xsl"));
                    xtCsv.Transform(xml, null, Response.OutputStream);

                    break;
            }
            this.Response.End();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridTiempos();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            FormPanel1.Reset();

            LimpiaGridTiempos();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al Grid de Tiempos
        /// </summary>
        protected void LimpiaGridTiempos()
        {
            btnExportXML.Disabled = true;
            btnExportExcel.Disabled = true;
            btnExportCSV.Disabled = true;

            HttpContext.Current.Session.Add("DtTiempos", null);
            StoreTiempos.RemoveAll();
        }

        protected void LlenaSucursales()
        {
            try
            {
                StoreSucursal.RemoveAll();
               
                var usert = this.Usuario;
                var id = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

                StoreSucursal.DataSource = DAOReportes.ListaSucursalesMoshi(usert, id);
                StoreSucursal.DataBind();
            }

            catch (Exception err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Tiempos de Reparto", "Ocurrió un Error al Obtener la Lista de Sucursales").Show();
            }
        }


        protected void LLenaTiposServicio()
        {
            try
            {
                var usert = this.Usuario;
                var id = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

                StoreTipoServicio.DataSource = DAOReportes.ListaTiposServicio(usert, id);
                StoreTipoServicio.DataBind();
            }

            catch (Exception err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Tiempos de Reparto", "Ocurrió un Error al Obtener la Lista de Tipos de Servicio").Show();
            }
        }


        protected void LLenaRepartidores()
        {
            try
            {
                var usert = this.Usuario;
                var id = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

                StoreRepartidor.DataSource = DAOReportes.ListaRepartidores(usert, id);
                StoreRepartidor.DataBind();
            }

            catch (Exception err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Tiempos de Reparto", "Ocurrió un Error al Obtener los Repartidores").Show();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de tiempos de reparto
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreTiempos_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelTiempos(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepTiemposRepartoMoshi")]
        public void ClicDePaso()
        {
            btnDownloadHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void Download(object sender, DirectEventArgs e)
        {
            ExportDataTableToExcel();
        }

        /// <summary>
        /// Exporta el reporte previamento consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepTiemposRepartoMoshi")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteTiemposReparto";
                DataTable _dtTiempos = HttpContext.Current.Session["DtTiempos"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtTiempos, reportName);

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

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Tiempos de Reparto", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 9).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 10).SetDataType(XLCellValues.Number);
            }

            return ws;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepTiemposRepartoMoshi")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}