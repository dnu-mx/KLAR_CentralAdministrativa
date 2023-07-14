using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.LogicaNegocio;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using Ext.Net;
using Interfases.Exceptiones;
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

namespace TpvWeb
{
    public partial class Reporte_OperacionesConSaldo : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.datInicio.SetValue(DateTime.Now);
                    this.datInicio.MaxDate = DateTime.Today;

                    this.datFinal.SetValue(DateTime.Now);
                    this.datFinal.MaxDate = DateTime.Today;

                    PagingOpsConSaldo.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtOpsConSaldo", null);

                    LlenaCombos();
                }
            }
            catch (Exception err)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Llena los controles de la página de tipo combo con la información de catálogos en base de datos
        /// </summary>
        protected void LlenaCombos()
        {
            DataSet dsCadenas = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, 
                Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), 
                Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                "ClaveTipoColectivaCadenaComercial").Valor, "", -1);

            List<ColectivaComboPredictivo> ListaCadenas = new List<ColectivaComboPredictivo>();

            foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
            {
                var ComboCadenas = new ColectivaComboPredictivo()
                {
                    ID_Colectiva = Convert.ToInt64(cadena["ID_Colectiva"].ToString()),
                    ClaveColectiva = cadena["ClaveColectiva"].ToString(),
                    NombreORazonSocial = cadena["NombreORazonSocial"].ToString()
                };
                ListaCadenas.Add(ComboCadenas);
            }

            this.StoreCadenaComercial.DataSource = ListaCadenas;
            this.StoreCadenaComercial.DataBind();
            
            this.StoreEstatus.DataSource = DAOCatalogos.ListaEstatusOperacion(new LogHeader());
            this.StoreEstatus.DataBind();

            this.StoreBeneficiario.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                "ClaveTipoColectivaBeneficiario").Valor, "0", -1);
            this.StoreBeneficiario.DataBind();
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Cadena para consultar y establecer
        /// la lista de sucursales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void LlenaSucursales(object sender, EventArgs e)
        {
            try
            {
                this.StoreSucursal.RemoveAll();
                
                this.StoreSucursal.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), 
                    Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "ClaveTipoColectivaSucursal").Valor, "0", Int64.Parse(cmbCadenaComercial.Value.ToString()));
                this.StoreSucursal.DataBind();
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Sucursales", err.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Sucursal para consultar y establecer
        /// la lista de afiliaciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void LlenaAfiliaciones(object sender, EventArgs e)
        {
            try
            {
                this.StoreAfiliacion.RemoveAll();
                
                this.StoreAfiliacion.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), 
                    Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "ClaveTipoColectivaAfiliacion").Valor, cmbSucursal.Value.ToString(),
                    Int64.Parse(cmbCadenaComercial.Value.ToString()));
                this.StoreAfiliacion.DataBind();
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Afiliaciones", err.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Afiliación para consultar y establecer
        /// la lista de terminales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void LlenaTerminales(object sender, EventArgs e)
        {
            try
            {
                this.StoreTerminal.RemoveAll();

                this.StoreTerminal.DataSource = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "ClaveTipoColectivaTerminal").Valor, cmbAfiliacion.Value.ToString(),
                    Int64.Parse(cmbCadenaComercial.Value.ToString()));
                this.StoreTerminal.DataBind();
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Terminales", err.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Terminal para consultar y establecer
        /// la lista de operadores
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void LlenaOperadores(object sender, EventArgs e)
        {
            try
            {
                this.StoreOperador.RemoveAll();

                this.StoreOperador.DataSource = DAOCatalogos.ListaTiposColectivasPorTipoColectiva(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "ClaveTipoColectivaOperador").Valor);
                this.StoreOperador.DataBind();
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operadores", err.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar reestableciendo los controles,
        /// páneles y grids a su estado de carga inicial
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.FormOpsConSaldo.Reset();

            LimpiaGridOpsConSaldo();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones
        /// </summary>
        protected void LimpiaGridOpsConSaldo()
        {
            this.btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtOpsConSaldo", null);
            this.StoreOpsConSaldo.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridOpsConSaldo();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de operaciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreOpsConSaldo_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridOpsConSaldo(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de operaciones, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridOpsConSaldo(int RegistroInicial, string Columna, SortDirection Orden)
        {
            String Afiliacion, Sucursal, Terminal, Operador, Estatus, Telefono, Benefiario;
            DateTime fechaInicial, FechaFinal;
            int ID_CadenaComercial;
            this.btnExportExcel.Disabled = true;

            try
            {
                DataTable dtOpsConSaldo = new DataTable();

                dtOpsConSaldo = HttpContext.Current.Session["DtOpsConSaldo"] as DataTable;

                if (dtOpsConSaldo == null)
                {
                    fechaInicial = this.datInicio.SelectedDate;
                    FechaFinal = this.datFinal.SelectedDate;

                    ID_CadenaComercial = cmbCadenaComercial.Value == null ? -1 : int.Parse(cmbCadenaComercial.Value.ToString());
                    Afiliacion = cmbAfiliacion.Value == null ? "-1" : cmbAfiliacion.Value.ToString();
                    Sucursal = cmbSucursal.Value == null ? "-1" : cmbSucursal.Value.ToString();
                    Terminal = cmbTerminal.Value == null ? "-1" : cmbTerminal.Value.ToString();
                    Operador = cmbOperador.Value == null ? "-1" : cmbOperador.Value.ToString();
                    Estatus = cmbEstatus.Value == null ? "-1" : cmbEstatus.Value.ToString();
                    Benefiario = cmbBeneficiario.Value == null ? "-1" : cmbBeneficiario.Value.ToString();
                    Telefono = txtTelefono.Text.Trim().Length == 0 ? "-1" : txtTelefono.Text.Trim();
                    
                    dtOpsConSaldo = DAOReportes.ObtieneOperacionesCadenaConSaldo(fechaInicial, FechaFinal, Sucursal,
                        Afiliacion, Terminal, Operador, Benefiario, Estatus, Telefono, ID_CadenaComercial, 
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                    HttpContext.Current.Session.Add("DtOpsConSaldo", dtOpsConSaldo);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtOpsConSaldo.Rows.Count < 1)
                {
                    X.Msg.Alert("Operaciones", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtOpsConSaldo.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Operaciones", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepOpsSaldo.ClicDePaso()",
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
                    int TotalRegistros = dtOpsConSaldo.Rows.Count;

                    (this.StoreOpsConSaldo.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtOpsConSaldo.Clone();
                    DataTable dtToGrid = dtOpsConSaldo.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtOpsConSaldo.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingOpsConSaldo.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingOpsConSaldo.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtOpsConSaldo.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreOpsConSaldo.DataSource = dtToGrid;
                    this.StoreOpsConSaldo.DataBind();

                    this.btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Operaciones", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", "Ocurrió un Error al Consultar el Reporte").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepOpsSaldo")]
        public void ClicDePaso()
        {
            this.btnDownloadHide.FireEvent("click");
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
        [DirectMethod(Namespace = "RepOpsSaldo")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "DetalleCadena";
                DataTable _dtOperaciones = HttpContext.Current.Session["DtOpsConSaldo"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtOperaciones, reportName);

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

                this.Response.End();
            }

            catch (Exception ex)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
            ws.Column(2).Hide();

            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.Number);
            }

            return ws;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepOpsSaldo")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}