using ClosedXML.Excel;
using DALCentralAplicaciones.Utilidades;
using DALLealtad.BaseDatos;
using DALLealtad.LogicaNegocio;
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
using System.Xml;
using System.Xml.Xsl;

namespace Lealtad
{
    public partial class ReporteImagenesCadenas : DALCentralAplicaciones.PaginaBaseCAPP
    {
        public class CadenaComboPredictivo
        {
            public Int64 ID_Cadena { get; set; }
            public String ClaveCadena { get; set; }
            public String NombreComercial { get; set; }
        }

        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Imagenes
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    HttpContext.Current.Session.Add("DtCadenas", null);

                    DataSet dsCadenas = DAOEcommercePrana.ListaCadenas(this.Usuario);

                    List<CadenaComboPredictivo> ComboList = new List<CadenaComboPredictivo>();

                    foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
                    {
                        var cadenaCombo = new CadenaComboPredictivo()
                        {
                            ID_Cadena = Convert.ToInt32(cadena["ID_Cadena"].ToString()),
                            ClaveCadena = cadena["ClaveCadena"].ToString(),
                            NombreComercial = cadena["NombreComercial"].ToString()
                        };
                        ComboList.Add(cadenaCombo);
                    }

                    StoreClaveCadena.DataSource = ComboList;
                    StoreClaveCadena.DataBind();

                    HttpContext.Current.Session.Add("DtCadenas", dsCadenas.Tables[0]);


                    PagingToolBar1.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtImagenes", null);
                }

                if (!X.IsAjaxRequest)
                {

                }
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            cBoxClaveCadena.Clear();
            txtCadena.Clear();

            LimpiaGridImagenes();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al Grid de Imágenes
        /// </summary>
        protected void LimpiaGridImagenes()
        {
            this.btnExportExcel.Disabled = true;
            this.btnExportCSV.Disabled = true;

            HttpContext.Current.Session.Add("DtImagenes", null);
            StoreImagenes.RemoveAll();
        }

        /// <summary>
        /// Añade las columnas del DataTable que va al grid
        /// </summary>
        /// <param name="_dtImgs">Objeto DataTable ya creado</param>
        protected void AgregaColumnas_DtImagenes(DataTable _dtImgs)
        {
            _dtImgs.Columns.Add("ID_Cadena");
            _dtImgs.Columns.Add("ClaveCadena");
            _dtImgs.Columns.Add("NombreComercial");
            _dtImgs.Columns.Add("LogoSams");
            _dtImgs.Columns.Add("LogoEdenred");
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de impagenes, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelImagenes(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;
            this.btnExportCSV.Disabled = true;

            try
            {
                DataTable dtImagenes = new DataTable();

                dtImagenes = HttpContext.Current.Session["DtImagenes"] as DataTable;

                if (dtImagenes == null)
                {
                    dtImagenes = new DataTable();
                    DataTable _dtCadenas = HttpContext.Current.Session["DtCadenas"] as DataTable;

                    if (cBoxClaveCadena.SelectedItem.Value == "-1")
                    {
                        dtImagenes = _dtCadenas;

                        BuscaImagenesCadenas(dtImagenes, true);
                    }
                    else
                    {
                        AgregaColumnas_DtImagenes(dtImagenes);

                        if (!String.IsNullOrEmpty(cBoxClaveCadena.SelectedItem.Value))
                        {
                            dtImagenes.Rows.Add();

                            dtImagenes.Rows[0]["ID_Cadena"] = cBoxClaveCadena.SelectedItem.Value;
                            dtImagenes.Rows[0]["ClaveCadena"] = hdnClaveCadena.Value;
                            dtImagenes.Rows[0]["NombreComercial"] = hdnNombreCadena.Value;

                            BuscaImagenesCadenas(dtImagenes, false);
                        }
                        else
                        {
                            DataRow[] clavesCadenas = _dtCadenas.Select(string.Format(
                                "{0} LIKE '%{1}%'", "NombreComercial", txtCadena.Text));

                            for (int i = 0; i < clavesCadenas.Length; i++)
                            {
                                dtImagenes.Rows.Add();

                                dtImagenes.Rows[i]["ID_Cadena"] = clavesCadenas[i][0];
                                dtImagenes.Rows[i]["ClaveCadena"] = clavesCadenas[i][1];
                                dtImagenes.Rows[i]["NombreComercial"] = clavesCadenas[i][2];
                            }

                            BuscaImagenesCadenas(dtImagenes, true);
                        }
                    }

                    HttpContext.Current.Session.Add("DtImagenes", dtImagenes);
                }


                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtImagenes.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Imagenes", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepImagenes.ClicDePaso()",
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
                    int TotalRegistros = dtImagenes.Rows.Count;

                    (this.StoreImagenes.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtImagenes.Clone();
                    DataTable dtToGrid = dtImagenes.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtImagenes.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingToolBar1.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtImagenes.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreImagenes.DataSource = dtToGrid;
                    StoreImagenes.DataBind();

                    this.btnExportExcel.Disabled = false;
                    this.btnExportCSV.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Imagenes", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Imagenes", "Ocurrió un Error al Consultar el Reporte de Imagenes").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, DirectEventArgs e)
        {
            if (String.IsNullOrEmpty(cBoxClaveCadena.SelectedItem.Value) &&
                String.IsNullOrEmpty(txtCadena.Text))
            {
                X.Msg.Alert("Imagenes", "Selecciona al menos un criterio de búsqueda").Show();
                return;
            }

            LimpiaGridImagenes();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla la búsqueda de imágenes de las cadenas, ya sea para todas ellas
        /// o sólo para una, estableciendo el resultado en el DataTable.
        /// </summary>
        /// <param name="dt">DataTable con las claves de cadena por buscar</param>
        /// <param name="todas">Bandera para buscar todas las cadenas o no</param>
        protected void BuscaImagenesCadenas(DataTable dt, bool todas)
        {
            try
            {
                string consecutivo, claveEntregador, columna, extension;
                int numEntregadores = int.Parse(Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "FTP_NumEntregadores").Valor.ToString());

                for (int entregador = 1; entregador <= numEntregadores; entregador++)
                {
                    consecutivo = "FTP_" + entregador.ToString();

                    claveEntregador = Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        consecutivo).Valor.ToString();

                    columna = Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "FTP_" + claveEntregador + "_Columna").Valor.ToString();

                    extension = Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "FTP_" + claveEntregador + "_Extension").Valor.ToString();

                    if (todas)
                    {
                        LNConexionFTP.ValidaArchivosCadenas(claveEntregador, columna, extension, dt);
                    }
                    else
                    {
                        dt.Rows[0][columna] = LNConexionFTP.VerificaSiExisteArchivoImagen(
                        claveEntregador, hdnClaveCadena.Value + extension) ? "SI" : "NO";
                    }
                }
            }
            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento SUBMIT al querer exportar al formato seleccionado
        /// los resultados de la consulta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void StoreSubmit(object sender, StoreSubmitDataEventArgs e)
        {
            string format = this.FormatType.Value.ToString();

            XmlNode xml = e.Xml;

            this.Response.Clear();

            switch (format)
            {
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

        /// <summary>
        /// Controla el evento onRefresh del grid de beneficios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreImagenes_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelImagenes(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepImagenes")]
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
        [DirectMethod(Namespace = "RepImagenes")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteImagenes";
                DataTable _dtImagenes = HttpContext.Current.Session["DtImagenes"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtImagenes, reportName);

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
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Imagenes", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
            ws.Column(1).Hide();
            return ws;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepImagenes")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}