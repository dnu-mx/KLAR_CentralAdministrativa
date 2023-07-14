using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using Ext.Net;
using Interfases.Exceptiones;
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
    public partial class ConciliacionesSmartPoints : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Administrar Colectivas para Parabilia
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    HttpContext.Current.Session.Add("DtOpsConciliadas", null);
                    HttpContext.Current.Session.Add("MaxRegsOpsConc", 0);
                    HttpContext.Current.Session.Add("Flag_MaxRegsOpsConc", 0);

                    HttpContext.Current.Session.Add("DtOpsSiANoP", null);
                    HttpContext.Current.Session.Add("MaxRegsOpsSiANoP", 0);
                    HttpContext.Current.Session.Add("Flag_MaxRegsOpsSiANoP", 0);

                    HttpContext.Current.Session.Add("DtOpsNoASiP", null);
                    HttpContext.Current.Session.Add("MaxRegsOpsNoASiP", 0);
                    HttpContext.Current.Session.Add("Flag_MaxRegsOpsNoASiP", 0);
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del panel izquierdo, limpiando los controles,
        /// páneles y grids asociados a alguna previa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.FormPanelBusqueda.Reset();

            this.StoreArchivos.RemoveAll();

            LimpiaSeleccionPrevia();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de
        /// una colectiva en el Grid de Resultados
        /// </summary>
        protected void LimpiaSeleccionPrevia()
        {
            LimpiaGridOpsConciliadas();

            LimpiaGridOpsSiANoP();

            LimpiaGridOpsNoASiP();

            this.FormPanelOpsConc.Show();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones conciliadas
        /// </summary>
        protected void LimpiaGridOpsConciliadas()
        {
            this.btnExcelOpsConc.Disabled = true;

            HttpContext.Current.Session.Add("DtOpsConciliadas", null);
            HttpContext.Current.Session.Add("MaxRegsOpsConc", 0);
            HttpContext.Current.Session.Add("Flag_MaxRegsOpsConc", 0);

            this.StoreOpsConciliadas.RemoveAll();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones 
        /// sí en archivo/no en plataforma
        /// </summary>
        protected void LimpiaGridOpsSiANoP()
        {
            this.btnExcelOpsSiANoP.Disabled = true;

            HttpContext.Current.Session.Add("DtOpsSiANoP", null);
            HttpContext.Current.Session.Add("MaxRegsOpsSiANoP", 0);
            HttpContext.Current.Session.Add("Flag_MaxRegsOpsSiANoP", 0);

            this.StoreOpsSiANoP.RemoveAll();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones
        /// no en archivo/sí en plataforma
        /// </summary>
        protected void LimpiaGridOpsNoASiP()
        {
            this.btnExcelOpsNoASiP.Disabled = true;

            HttpContext.Current.Session.Add("DtOpsNoASiP", null);
            HttpContext.Current.Session.Add("MaxRegsOpsNoASiP", 0);
            HttpContext.Current.Session.Add("Flag_MaxRegsOpsNoASiP", 0);

            this.StoreOpsNoASiP.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel izquierdo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenaGridResultados();
        }

        /// <summary>
        /// Llena el grid de resultados con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            try
            {
                LimpiaSeleccionPrevia();

                DataSet dsArchivos = DAOConciliaciones.ObtieneArchivosSmartPoints(
                    this.txtNombreArchivo.Text, Convert.ToDateTime(this.dfFechaArchivo.SelectedDate),
                    String.IsNullOrEmpty(this.cBoxEstatusArchivo.SelectedItem.Value) ? -1 :
                    Convert.ToInt32(this.cBoxEstatusArchivo.SelectedItem.Value),
                    Convert.ToDateTime(this.dfFechaProc.SelectedDate), this.Usuario);

                if (dsArchivos.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Archivos", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    this.StoreArchivos.DataSource = dsArchivos;
                    this.StoreArchivos.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Archivos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Archivos", ex.Message).Show();
            }
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
                string NombreArchivo = String.Empty;

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] archivo = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (archivo == null || archivo.Length < 1)
                {
                    return;
                }

                foreach (KeyValuePair<string, string> column in archivo[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Fichero": this.hdnIdArchivo.Value = column.Value; break;
                        case "NombreFichero": NombreArchivo = column.Value; break;
                        default:
                            break;
                    }
                }

                LimpiaSeleccionPrevia();

                this.hdnConsultaWhere.Value = DAOConciliaciones.ObtieneWhereSmartPoints(this.Usuario);

                PanelCentral.Title += " Archivo " + NombreArchivo;

                this.FormPanelOpsConc.Show();
                this.btnLlenaGrids_Hide.FireEvent("click");
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Archivos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                X.Msg.Alert("Archivos", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de operaciones conciliadas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreOpsConciliadas_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridOpsConciliadas(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de operaciones conciliadas, así como el ordenamiento
        /// y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridOpsConciliadas(int RegistroInicial, string Columna, SortDirection Orden)
        {
            Int64 IdArchivo = Convert.ToInt64(this.hdnIdArchivo.Value);
            int RegistroFinal = 0, TotalRegistros = 0;

            this.btnExcelOpsConc.Disabled = true;
            this.StoreOpsConciliadas.RemoveAll();

            try
            {
                DataTable dtOpsConciliadas = new DataTable();

                dtOpsConciliadas = HttpContext.Current.Session["DtOpsConciliadas"] as DataTable;

                if (dtOpsConciliadas == null)
                {
                    dtOpsConciliadas = DAOConciliaciones.ObtieneOpsConciliadas_SmartPoints(
                        this.hdnConsultaWhere.Value.ToString(), IdArchivo, this.Usuario);

                    HttpContext.Current.Session.Add("DtOpsConciliadas", dtOpsConciliadas);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);
                TotalRegistros = dtOpsConciliadas.Rows.Count;

                DataTable sortedDT = dtOpsConciliadas.Clone();
                DataTable dtToGrid = dtOpsConciliadas.Clone();

                if (TotalRegistros > maxRegistros)
                {
                    HttpContext.Current.Session.Add("MaxRegsOpsConc", 1);

                    (this.StoreOpsConciliadas.Proxy[0] as PageProxy).Total = maxRegistros;

                    DataTable dtRecortada = dtOpsConciliadas.AsEnumerable().Take(maxRegistros).CopyToDataTable();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtRecortada.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    RegistroFinal = (RegistroInicial + PagingTBOpsConc.PageSize) < maxRegistros ?
                        (RegistroInicial + PagingTBOpsConc.PageSize) : maxRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtRecortada.Rows[row] : sortedDT.Rows[row]);
                    }
                }
                else
                {
                    (this.StoreOpsConciliadas.Proxy[0] as PageProxy).Total = TotalRegistros;

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtOpsConciliadas.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    RegistroFinal = (RegistroInicial + PagingTBOpsConc.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingTBOpsConc.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtOpsConciliadas.Rows[row] : sortedDT.Rows[row]);
                    }
                }

                dtToGrid.AcceptChanges();

                this.StoreOpsConciliadas.DataSource = dtToGrid;
                this.StoreOpsConciliadas.DataBind();

                this.btnExcelOpsConc.Disabled = TotalRegistros > 0 ? false : true;
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Operaciones Conciliadas", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                DALAutorizador.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones Conciliadas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento clic al botón Exportar a Excel de la pestaña operaciones conciliadas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnExcelOpsConc_Click(object sender, DirectEventArgs e)
        {
            try
            {
                string reportName = "OperacionesConciliadas";
                DataTable _dtOpsSiANoP = HttpContext.Current.Session["DtOpsConciliadas"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtOpsSiANoP, reportName);

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
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones Conciliadas", "Ocurrió un error al exportar a Excel").Show();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de operaciones sí en archivo/no en plataforma
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreOpsSiANoP_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridOpsSiANoP(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de operaciones sí en archivo/no en plataforma,
        /// así como el ordenamiento y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridOpsSiANoP(int RegistroInicial, string Columna, SortDirection Orden)
        {
            Int64 IdArchivo = Convert.ToInt64(this.hdnIdArchivo.Value);
            int RegistroFinal = 0, TotalRegistros = 0;

            this.btnExcelOpsSiANoP.Disabled = true;
            this.StoreOpsSiANoP.RemoveAll();

            try
            {
                DataTable dtOpsSiANoP = new DataTable();

                dtOpsSiANoP = HttpContext.Current.Session["DtOpsSiANoP"] as DataTable;

                if (dtOpsSiANoP == null)
                {
                    dtOpsSiANoP = DAOConciliaciones.ObtieneOpsSiANoP_SmartPoints(
                        this.hdnConsultaWhere.Value.ToString(), IdArchivo, this.Usuario);

                    HttpContext.Current.Session.Add("DtOpsSiANoP", dtOpsSiANoP);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);
                TotalRegistros = dtOpsSiANoP.Rows.Count;

                DataTable sortedDT = dtOpsSiANoP.Clone();
                DataTable dtToGrid = dtOpsSiANoP.Clone();

                if (TotalRegistros > maxRegistros)
                {
                    HttpContext.Current.Session.Add("MaxRegsOpsSiANoP", 1);

                    (this.StoreOpsSiANoP.Proxy[0] as PageProxy).Total = maxRegistros;

                    DataTable dtRecortada = dtOpsSiANoP.AsEnumerable().Take(maxRegistros).CopyToDataTable();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtRecortada.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    RegistroFinal = (RegistroInicial + PagingTBOpsSiANoP.PageSize) < maxRegistros ?
                        (RegistroInicial + PagingTBOpsSiANoP.PageSize) : maxRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtRecortada.Rows[row] : sortedDT.Rows[row]);
                    }
                }
                else
                {
                    (this.StoreOpsSiANoP.Proxy[0] as PageProxy).Total = TotalRegistros;

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtOpsSiANoP.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    RegistroFinal = (RegistroInicial + PagingTBOpsSiANoP.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingTBOpsSiANoP.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtOpsSiANoP.Rows[row] : sortedDT.Rows[row]);
                    }
                }

                dtToGrid.AcceptChanges();

                this.StoreOpsSiANoP.DataSource = dtToGrid;
                this.StoreOpsSiANoP.DataBind();

                this.btnExcelOpsSiANoP.Disabled = TotalRegistros > 0 ? false : true;

            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Operaciones en Archivo, no en Plataforma", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones en Archivo, no en Plataforma", ex.Message).Show();
            }
        }
        
        /// <summary>
        /// Controla el evento clic al botón Exportar a Excel de la pestaña de
        /// operaciones sí en archivo/no en plataforma
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnExcelOpsSiANoP_Click(object sender, DirectEventArgs e)
        {
            try
            {
                string reportName = "OperSiArchNoPlat";
                DataTable _dtOpsSiANoP = HttpContext.Current.Session["DtOpsSiANoP"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtOpsSiANoP, reportName);

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
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones en Archivo, no en Plataforma", "Ocurrió un error al exportar a Excel").Show();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de operaciones no en archivo/sí en plataforma
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreOpsNoASiP_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;
            
            LlenaGridOpsNoASiP(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de operaciones no en archivo/no en plataforma,
        /// así como el ordenamiento y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridOpsNoASiP(int RegistroInicial, string Columna, SortDirection Orden)
        {
            Int64 IdArchivo = Convert.ToInt64(this.hdnIdArchivo.Value);
            int RegistroFinal = 0, TotalRegistros = 0;

            this.btnExcelOpsNoASiP.Disabled = true;
            this.StoreOpsNoASiP.RemoveAll();

            try
            {
                DataTable dtOpsNoASiP = new DataTable();

                dtOpsNoASiP = HttpContext.Current.Session["DtOpsNoASiP"] as DataTable;

                if (dtOpsNoASiP == null)
                {
                    dtOpsNoASiP = DAOConciliaciones.ObtieneOpsNoASiP_SmartPoints(
                        this.hdnConsultaWhere.Value.ToString(), IdArchivo, this.Usuario);

                    HttpContext.Current.Session.Add("DtOpsNoASiP", dtOpsNoASiP);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);
                TotalRegistros = dtOpsNoASiP.Rows.Count;

                DataTable sortedDT = dtOpsNoASiP.Clone();
                DataTable dtToGrid = dtOpsNoASiP.Clone();

                if (TotalRegistros > maxRegistros)
                {
                    HttpContext.Current.Session.Add("MaxRegsOpsNoASiP", 1);

                    (this.StoreOpsNoASiP.Proxy[0] as PageProxy).Total = maxRegistros;

                    DataTable dtRecortada = dtOpsNoASiP.AsEnumerable().Take(maxRegistros).CopyToDataTable();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtRecortada.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    RegistroFinal = (RegistroInicial + PagingTBOpsNoASiP.PageSize) < maxRegistros ?
                        (RegistroInicial + PagingTBOpsNoASiP.PageSize) : maxRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtRecortada.Rows[row] : sortedDT.Rows[row]);
                    }
                }
                else
                {
                    (this.StoreOpsNoASiP.Proxy[0] as PageProxy).Total = TotalRegistros;

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtOpsNoASiP.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    RegistroFinal = (RegistroInicial + PagingTBOpsNoASiP.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingTBOpsNoASiP.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtOpsNoASiP.Rows[row] : sortedDT.Rows[row]);
                    }
                }

                dtToGrid.AcceptChanges();

                this.StoreOpsNoASiP.DataSource = dtToGrid;
                this.StoreOpsNoASiP.DataBind();

                this.btnExcelOpsNoASiP.Disabled = TotalRegistros > 0 ? false : true;
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Operaciones en Plataforma, no en Archivo", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones en Plataforma, no en Archivo", ex.Message).Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento clic al botón Exportar a Excel de la pestaña de
        /// operaciones no en archivo/sí en plataforma
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnExcelOpsNoASiP_Click(object sender, DirectEventArgs e)
        {
            try
            {
                string reportName = "OperNoArchSiPlat";
                DataTable _dtOpsNoASiP = HttpContext.Current.Session["DtOpsNoASiP"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtOpsNoASiP, reportName);

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
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones en Plataforma, no en Archivo", "Ocurrió un error al exportar a Excel").Show();
            }
        }

        /// <summary>
        /// Controla el evento DataBinding del StoreOpsConciliadas, para controlar
        /// el mensaje de exceso de registros en la consulta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void StoreOpsConciliadas_Msg(object sender, EventArgs e)
        {
            int NumMaxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

            bool OpsConc = Convert.ToBoolean(HttpContext.Current.Session["MaxRegsOpsConc"]);
            bool flag = Convert.ToBoolean(HttpContext.Current.Session["Flag_MaxRegsOpsConc"]);

            if (OpsConc && !flag)
            {
                HttpContext.Current.Session.Add("Flag_MaxRegsOpsConc", 1);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Operaciones",
                    Message = "La búsqueda arrojó demasiadas coincidencias en una o más pestañas. Se muestran sólo " +
                        "las primeras <b> " + NumMaxRegistros.ToString() + "</b> operaciones.</br> Si requieres todas " +
                        "las operaciones, exporta el archivo Excel correspondiente.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO
                });
            }
        }

        /// <summary>
        /// Controla el evento DataBinding del StoreOpsSiANoP, para controlar
        /// el mensaje de exceso de registros en la consulta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void StoreOpsSiANoP_Msg(object sender, EventArgs e)
        {
            int NumMaxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

            bool OpsSiANoP = Convert.ToBoolean(HttpContext.Current.Session["MaxRegsOpsSiANoP"]);
            bool flagOpsSiANoP = Convert.ToBoolean(HttpContext.Current.Session["Flag_MaxRegsOpsSiANoP"]);

            if (OpsSiANoP && !flagOpsSiANoP)
            {
                HttpContext.Current.Session.Add("Flag_MaxRegsOpsSiANoP", 1);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Operaciones",
                    Message = "La búsqueda arrojó demasiadas coincidencias en una o más pestañas. Se muestran sólo " +
                        "las primeras <b> " + NumMaxRegistros.ToString() + "</b> operaciones.</br> Si requieres todas " +
                        "las operaciones, exporta el archivo Excel correspondiente.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO
                });
            }
        }

        /// <summary>
        /// Controla el evento DataBinding del StoreOpsNoASiP, para controlar
        /// el mensaje de exceso de registros en la consulta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void StoreOpsNoASiP_Msg(object sender, EventArgs e)
        {
            int NumMaxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

            bool OpsNoASiP = Convert.ToBoolean(HttpContext.Current.Session["MaxRegsOpsNoASiP"]);
            bool flagOpsNoASiP = Convert.ToBoolean(HttpContext.Current.Session["Flag_MaxRegsOpsNoASiP"]);

            if (OpsNoASiP && !flagOpsNoASiP)
            {
                HttpContext.Current.Session.Add("Flag_MaxRegsOpsNoASiP", 1);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Operaciones",
                    Message = "La búsqueda arrojó demasiadas coincidencias en una o más pestañas. Se muestran sólo " +
                        "las primeras <b> " + NumMaxRegistros.ToString() + "</b> operaciones.</br> Si requieres todas " +
                        "las operaciones, exporta el archivo Excel correspondiente.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO
                });
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
            return ws;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "ConcSP")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}
