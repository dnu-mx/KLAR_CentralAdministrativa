using ClosedXML.Excel;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Utilidades;
using DALLealtad.BaseDatos;
using DALLealtad.Entidades;
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

namespace Lealtad
{
    public partial class GenerarCodigosCupones : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    dfExpiracion.SetValue(DateTime.Now.AddMonths(1));
                    dfExpiracion.MinDate = DateTime.Today;

                    LlenaComboCadenas();
                    LlenaComboPromociones();

                    PagingToolBar1.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtCodigos", null);
                }
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Llena el combo con cadenas comerciales
        /// </summary>
        protected void LlenaComboCadenas()
        {
            try
            {
                DataSet dsCadenas = DAOPromociones.ListaCadenasComerciales(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                List<ColectivaComboPredictivo> cadenaList = new List<ColectivaComboPredictivo>();

                foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
                {
                    var cadenaCombo = new ColectivaComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt64(cadena["ID_Colectiva"].ToString()),
                        ClaveColectiva = cadena["ClaveColectiva"].ToString(),
                        NombreORazonSocial = cadena["NombreORazonSocial"].ToString()
                    };
                    cadenaList.Add(cadenaCombo);
                }

                StoreCadenas.DataSource = cadenaList;
                StoreCadenas.DataBind();

                if (dsCadenas.Tables[0].Rows.Count == 1)
                {
                   cBoxCadena.SelectedIndex = 0;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consulta de Cadenas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Llena el combo con las promociones permitidas para el usuario y la aplicación
        /// </summary>
        protected void LlenaComboPromociones()
        {
            try
            {
                DataTable dtEventos = DAOPromociones.ListaEventosGenerarCodigos(Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveEventoGeneraCodigos").Valor,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                StoreEvento.DataSource = dtEventos;
                StoreEvento.DataBind();

                if (dtEventos.Rows.Count == 1)
                {
                    cBoxEvento.SelectedIndex = 0;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consulta de Promociones", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            FormPanel1.Reset();

            HttpContext.Current.Session.Add("DtBeneficios", null);

            LimpiaGridCodigosGenerados();
        }

        /// <summary>
        /// Controla el evento Click al botón de Generar Códigos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnGenerar_Click(object sender, DirectEventArgs e)
        {
            int Cantidad, Longitud;
            double Valor;

            try
            {
                Cantidad = int.Parse(txtCantidad.Text);

                if (Cantidad < 1 || Cantidad > 10000)
                {
                    throw new CAppException(8006, "La Cantidad de Códigos debe ser un número entre 1 y 10,000.");
                }

                Longitud = int.Parse(txtLongitud.Text);

                if (Longitud < 6 || Longitud > 20)
                {
                    throw new CAppException(8006, "La Longitud del Código debe ser un número entre 6 y 20.");
                }

                Valor = double.Parse(txtValor.Text);

                if (Valor <= 0 || Valor > 999999)
                {
                    throw new CAppException(8006, "El Valor en Puntos debe ser un número mayor a 0 y menor a 999,999.");
                }


                Cupon elCupon = new Cupon();

                elCupon.ClaveEvento = cBoxEvento.SelectedItem.Value;
                elCupon.CantidadCupones = Cantidad;
                elCupon.PromoCode = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "PromoCode_EvGC").Valor;
                elCupon.TipoEmision = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "TipoEmision_EvGC").Valor;
                elCupon.Algoritmo = cBoxTipoCodigo.SelectedItem.Value;
                elCupon.Longitud = Longitud;
                elCupon.FechaExpiracion = dfExpiracion.SelectedDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                elCupon.ValorCupon = Valor;
                elCupon.ConsumosValidos = int.Parse(Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "ConsumosValidos_EvGC").Valor);
                elCupon.ClaveCadenaComercial = cBoxCadena.SelectedItem.Value;
                elCupon.TipoCupon = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "TipoCupon_EvGC").Valor;

                hdnIdColectiva.Value = LNPromociones.CreaCuponesPromocionMasiva(elCupon, this.Usuario);

                X.Msg.Notify("Generación de Códigos", "Los códigos se generaron <br /><br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LimpiaGridCodigosGenerados();

                Thread.Sleep(100);

                btnBuscarHide.FireEvent("click");
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Generación de Códigos", caEx.Mensaje()).Show();

            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Generación de Códigos", err.Message).Show();
            }
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones
        /// </summary>
        protected void LimpiaGridCodigosGenerados()
        {
            btnExportExcel.Disabled = true;

            StoreCodigosGenerados.RemoveAll();
        }

        /// <summary>
        /// Llena el grid con los códigos recién generados
        /// </summary>
        protected void LlenaGridCodigosGenerados(int RegistroInicial, string Columna, SortDirection Orden)
        {
            btnExportExcel.Disabled = true;

            try
            {
                DataTable dtCodigos = new DataTable();

                dtCodigos = HttpContext.Current.Session["DtCodigos"] as DataTable;

                if (dtCodigos == null)
                {
                    dtCodigos = DAOPromociones.ConsultaCodigosGenerados(
                        cBoxEvento.SelectedItem.Value, Convert.ToInt64(hdnIdColectiva.Value), this.Usuario);

                    HttpContext.Current.Session.Add("DtCodigos", dtCodigos);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtCodigos.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Códigos Generados", "La generación arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "Los códigos se exportarán directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "GeneraCodigos.ClicDePaso()",
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
                    int TotalRegistros = dtCodigos.Rows.Count;

                    (this.StoreCodigosGenerados.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtCodigos.Clone();
                    DataTable dtToGrid = dtCodigos.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtCodigos.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingToolBar1.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtCodigos.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreCodigosGenerados.DataSource = dtToGrid;
                    StoreCodigosGenerados.DataBind();

                    btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Códigos Generados", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Códigos Generados", ex.Message).Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de códigos generados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreCodigosGenerados_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridCodigosGenerados(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "GeneraCodigos")]
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
        [DirectMethod(Namespace = "GeneraCodigos")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "CodigosGenerados";
                DataTable _dtCodigos = HttpContext.Current.Session["DtCodigos"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtCodigos, reportName);

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
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Códigos Generados", "Ocurrió un Error al Exportar los Códigos a Excel").Show();
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

            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
            }

            return ws;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "GeneraCodigos")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}